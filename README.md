# StockChat Solution
This Solution have the following projects:

**1. Stock Chat** This application is an ASP.net core MVC application that works as a chat room and could send a command to retrieve a Stock data from an external endpoint.

**2. Stock Api** This application is an ASP.net core API application that works as a endpoint to retrieve Stock data from a external endpoint, the data comes as a CSV file and after being processed is it sent to a **RabbitMQ queue**

**3. UnitTestProject** Small testing project for Stock Api.

## Requirements

**1. RabbitMQ** For proper work of this solution must be installed a RabbitMQ server locally or in a Docker container properly configured. It can be downloaded from the following url [RabbitMQ](https://www.rabbitmq.com/install-windows.html)
   - **Configuration**: After RabbitMQ being installed must be configured in the following way:
     - Create a Queue with the name **StockQueue**
     - Create a User with the name **stockQueueUser** with access to the virtual host "/" and:
       * Configure regexp: .*
       * Write regexp: .*
       * Read regexp: .*
       
       Give Access to the topic "/"
       * Write regexp: .*
       * Read regexp: .*
   - Could also import the definitions located in the file [rabbit_2019-9-29.json](rabbit_2019-9-29.json) which matches the used in development and those values are configured in the applications.
   - Also the RabbitMQ could run in a docker, the image is located in [RabbitMQ Docker Image](https://hub.docker.com/_/rabbitmq)

**2. ASP .Net Core 2.2.0** [Download](https://dotnet.microsoft.com/download/dotnet-core/2.2)

**3. SQL Server** To create the Identity Framework database but could use the In Memory database.

## Projects

## Stock Chat

This application is intended to works as a Chat Room for Stocks. It uses Identity Framework for all the users CRUD operations, before accessing a user should register to the application. 
The chat room is handled with SignalR. The chat hub receives all the incomming messages and handles them properly, if is a common message is treated as a regular chat message and send it back to all the users currently logged and in the chat room, if the message is a *command message* it's send to a bot endpoint which handle it's handled properly and the bot received response is sent as a message to a RabbitMQ Queue.

The command syntax is **/stock=stockid**.

The App is also being configured as a Queue Consumer, because of this all the messages received from the Queue are also handled properly and sent them back to all the clients existing in the Chat Room view. 

### Configuration

The file *appsettings.json* have the following entries that could be changed according to local requirements.
1. ConnectionStrings
   - DefaultConnection: This is the default database connection. Could be a SQL Server database or could be a in memory database, to the former the connection string should looks like *Server=**(localdb)\\mssqllocaldb**;Database=aspnet-StockChat;Trusted_Connection=True;MultipleActiveResultSets=true*
2. RabbitMQ
   - Host: RabbitMQ Server host name, for local MQ this should be *localhost*
   - User: RabbitMQ user
   - Password: RabbitMQ user password
   - QueueName: RabbitMQ Queue name used for the solution
   - QueueExchange: RabbitMQ Queue Exchange used for this solution
3. ApiBotUrl: API Bot URL used for handling the stock command.

### Considerations

* **Database Creation**
  If the application doesn't connect to the database, it could be because it isn't created or the data entities aren't created yet. If trying to login or register a new user the following image will appear:

![Database creation](/DatabaseCreation.png =561x292)

  After hitting "Apply Migrations" button the database will be created or populated with all the data strutures. The will appear the following image:

![Database creation finished](/DatabaseCreationFinish.png =553x295)
  Then just refresh the page.

* There's a self installation [StockChat.deploy.cmd](/StockChat/StockChat/Deploy/StockChat.deploy.cmd) that will extract all the required files to run the application, the *appsettings.json* file is located in the *StockChat.zip* file.

## Stock Bot

This application is a REST API application intended to receive a Stock Id and consume an external endpoint to get some Stock data like opening, high, low and close values for a given Stock ID. The data is received in a CSV format and then is parsed and processed as JSON document, this JSON then is sent to a RabbitMQ queue. This application is configured as a publisher of a RabbitMQ queue.

### Configuration

The file *appsettings.json* have the following entries that could be changed according to local requirements.
1. RabbitMQ
   - Host: RabbitMQ Server host name, for local MQ this should be *localhost*
   - User: RabbitMQ user
   - Password: RabbitMQ user password
   - QueueName: RabbitMQ Queue name used for the solution


There's a self installation file [StockApi.deploy.cmd](/StockChat/StockApi/Deploy/StockApi.deploy.cmd) that will extract all the required files to run the application, the *appsettings.json* file is located in the *StockApi.zip* file.