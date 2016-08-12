# Couchbase Compliance Demo with .NET

This project is meant to demonstrate a full stack application using .NET, Angular 2, and Couchbase Server.  This project shows separation of the frontend and backend code as well as highly relational, yet schema-less NoSQL document data.

## The Requirements

There are a few requirements that must be met before trying to run this project:

* The Angular 2 CLI
* Node.js 4.0+
* .NET 4.x
* Couchbase Server 4.1+ 

The Angular CLI is necessary to build the Angular 2 frontend that is provided with this project.  The backend will be built with ASP.NET WebAPI.  Couchbase Server is required as the database layer.

## Configuring the Project

There are three things that must be done to run this project:

1. The Angular 2 frontend must be built
2. The .NET project must be compiled
3. The database must be configured

The Angular 2 code is not dependent on .NET or the database.  To build the Angular 2 project, execute the following from the **angular** directory of the project:

```sh
npm install
```

The above commands will install the Angular 2 dependencies and build the **angular/src** to the **public** directory.

Before compiling the .NET code, the Couchbase Server information must be specified first.  In the project's **Web.config** file, take a look at the details:

```
<add key="CouchbaseServer" value="couchbase://192.168.1.5" />
<add key="CouchbaseBucket" value="comply" />
```

Adjust the host and bucket information to match that of your own.

To build the .NET project, open it in Visual Studio, and execute (F5/Ctrl+F5):

Once the .NET WebAPI project is running, you will need to tell the Angular project where to find the endpoints. Edit the utility.ts file and look in the makePostRequest and makeGetRequest methods.
Replace "http://localhost:NNNNN" with whatever URL you are running the WebAPI project in. If you are lauching from Visual Studio, this would still be localhost but your port number may vary. NOTE: In a production environment, this might be a
different domain, different subdomain, etc, from the where you are serving the Angular app from. There is no requirement that they exist on the same machine or even the same data center. (In fact,
the WebAPI code in this demo has CORS enabled for any origin).

Once that change is in place, build the Angular project and start serving it.

```sh
ng build —output-path=../public
ng server
```

This will be served on http://localhost:4200/

Because this project makes use of N1QL, the bucket that you define must have at least one index. This can be created by:

```
CREATE PRIMARY INDEX on `comply`
```

In a production environment, you would likely have several more secondary indexes to improve performance.

## Running the Project

The web browser should open automatically from Visual Studio, to an address like **http://localhost:12345**.

## Resources

Couchbase - [http://www.couchbase.com](http://www.couchbase.com)

Couchbase Compliance Demo with Node.js - [https://github.com/couchbaselabs/comply](https://github.com/couchbaselabs/comply)

Couchbase Compliance Demo with GoLang - [https://github.com/couchbaselabs/comply-golang](https://github.com/couchbaselabs/comply-golang)

Couchbase Compliance Demo with Java - [https://github.com/couchbaselabs/comply-java](https://github.com/couchbaselabs/comply-java)
