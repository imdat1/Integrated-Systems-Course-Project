### The Basics
This is a recreation of the [LLM Database Chatbot app](https://github.com/imdat1/LLM-Database-Chatbot-WebProgramming-Project). It's a recreation in .NET Core 8.0.

### It includes:
#### The Normal App
A normal app where a person can register with their HuggingFace API Token. If the token isn't valid, the registration won't continue. 

Adding Postgres databases while checking if their credentials are correct through the Python Flask app. If they're not, the creation of the database won't continue.

Chatting with those databases using the Python Flask app to generate queries via the HuggingFaceHub Model API Endpoints and run them on the database.

#### The Admin app
The admin app can list all registered users. There's also the option to import users via an Excel file in the following column format:

<table>
    <thead>
        <th>Email</th>
        <th>Password</th>
        <th>Confirm Password (Repeat pasword)</th>
        <th>HuggingFace API Token</th>
    </thead>
</table>

If the HuggingFace API token is entered incorrectly in the Excel file, that user will be skipped and the others will be added while giving you a list of incorrect users in your Excel file.

The admin app can also list the databases for a given user. Here, you can import databases for a user via an Excel file in the following column format:

<table>
    <thead>
        <th>Random Name For Personal Database Identification</th>
        <th>Database Username</th>
        <th>Database Password</th>
        <th>Database Host</th>
        <th>Database Name</th>
    </thead>
</table>

Here, the database credentials are also checked on the Python Flask app. If they're incorrect, that entry for a database gets skipped and the others are added. You'll also be shown which entries for databases specifically in your import Excel file are incorrect. Keep in mind that if you try to import duplicate databases (import them twice), they won't be imported for the given user if they already exist.

In this section you can also export the questions and answers for a given database which has been asked by a given user into an Excel file. 

Some testing Excel files for users and databases can be found in the root folder of the Admin app. 

### How to Start
You can start both of these apps with Visual Studio, just keep in mind that the exposed ports of the normal app might be different in the admin app which communicates with it so you might need to change them.

Don't forget to "add-migration" and/or "update-database". For this, the AppDbContext is in the "Repository" of the normal app.

The Python Flask app can be started through a Docker container by running the following command:

"docker run -p 5000:5000 imdat1/flask_app_integrated"

For testing purposes, I can also suggest running the [bradymholt/postgres-northwind:latest](https://github.com/bradymholt/docker-postgresql-northwind) Docker image for starting a Postgres SQL server which has the Northwind testing database included. All the credentials for the database are "northwind", except for the host which depends on which port you try to run it on. I'd suggest just running this command:

"docker run --rm -it -p 5433:5432 --name postgres-northwind bradymholt/postgres-northwind:latest"


