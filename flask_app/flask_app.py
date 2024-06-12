from flask import Flask, request, jsonify
import os
import numpy as np
import pandas as pd

from langchain import SQLDatabase
from langchain_community.agent_toolkits import create_sql_agent

from langchain_core.prompts import ChatPromptTemplate
from langchain_core.output_parsers import StrOutputParser
from langchain_core.runnables import RunnablePassthrough

from langchain_community.llms import HuggingFaceEndpoint
from langchain_community.llms import HuggingFaceHub
from langchain.chains import LLMChain
from langchain.prompts import PromptTemplate

from sqlalchemy.exc import InterfaceError, DatabaseError, DataError, OperationalError, IntegrityError, InternalError, ProgrammingError, NotSupportedError

app = Flask(__name__)

import environ
env = environ.Env()
environ.Env.read_env()

llm = None


@app.route('/connect', methods=['POST'])
def connect_to_database():
    def connect_to_database(db_username, db_pass, db_host, db_name):
        try:
            input_db = SQLDatabase.from_uri(
                f"postgresql+psycopg2://{db_username}:{db_pass}@{db_host}/{db_name}"
            )
            return True
        except (InterfaceError, DatabaseError, DataError, OperationalError, IntegrityError, InternalError, ProgrammingError, NotSupportedError):
            return False
    # Get database credentials from the request
    db_username = request.form.get('dbUsername')
    db_pass = request.form.get('dbPass')
    db_host = request.form.get('dbHost')
    db_name = request.form.get('dbName')

    # Attempt to connect to the database
    connection_success = connect_to_database(db_username, db_pass, db_host, db_name)

    if connection_success:
        return jsonify({'status': 'success', 'message': 'Database connection successful'}), 200
    else:
        return jsonify({'status': 'error', 'message': 'Failed to connect to the database'}), 400


@app.route('/ask', methods=['POST'])
def ask_question():
    global input_db, llm
    
    # Get database credentials from the request
    db_username = request.form.get('dbUsername')
    db_pass = request.form.get('dbPass')
    db_host = request.form.get('dbHost')
    db_name = request.form.get('dbName')
    huggingface_token = request.form.get('huggingface_token')
    question = request.form.get('question')

    # Establish a connection to the PostgreSQL database
    input_db = SQLDatabase.from_uri(
        f"postgresql+psycopg2://{db_username}:{db_pass}@{db_host}/{db_name}",
    )

    template = """Based on the table schema below, write a SQL query that would answer the user's question:
    {schema}

    Question: {question}
    SQL Query:"""
    prompt = ChatPromptTemplate.from_template(template)

    def get_schema(db):
        schema = input_db.get_table_info()
        return schema

    repo_id = "mistralai/Mistral-7B-Instruct-v0.3"

    llm = HuggingFaceEndpoint(
        repo_id=repo_id, temperature=0.5, huggingfacehub_api_token=huggingface_token
    )

    sql_chain = (
        RunnablePassthrough.assign(schema=get_schema)
        | prompt
        | llm.bind(stop=["\nSQLResult:"])
        | StrOutputParser()
    )

    print(sql_chain.invoke({"question": question}))

    template = """Based on the table schema below, question, sql query, and sql response, write a natural language response:
    {schema}

    Question: {question}
    SQL Query: {query}
    SQL Response: {response}"""
    prompt_response = ChatPromptTemplate.from_template(template)

    def run_query(query):
        return input_db.run(query)

    full_chain = (
        RunnablePassthrough.assign(query=sql_chain).assign(
            schema=get_schema,
            response=lambda vars: run_query(vars["query"]),
        )
        | prompt_response
        | llm
    )
    answer = full_chain.invoke({"question": question})
    
    # Return the answer
    return jsonify({'answer': answer})

if __name__ == '__main__':
    app.run(debug=True)
