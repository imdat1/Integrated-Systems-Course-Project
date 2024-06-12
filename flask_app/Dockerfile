FROM python:3.9-slim-buster
WORKDIR /app
COPY ./requirements.txt /app
RUN pip install -r requirements.txt
RUN apt-get update \
    && apt-get -y install libpq-dev gcc \
    && pip install psycopg2
COPY . .
EXPOSE 5000
ENV FLASK_APP=flask_app.py
CMD ["flask", "run", "--host", "0.0.0.0"]