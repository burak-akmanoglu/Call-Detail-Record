# CDR Call Detail Record
+ My firt layer(MyTcpListener) is a Tcp/Port listener.This layer listens for incoming packets and saves them to the database.
+ Second layer(TcpListenerApi) is a API. The API layer connects with the database. And generates tokens for user control.
+ Third layer(TcpListenerWeb) is a Presentation Layer. This layer is connected to the API. And creates an excel file.