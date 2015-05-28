Hello Professor/Arthur,


Here is our project as it currently stands heading into the final presentation.

Our server and client are both written in C#, for ease of use with the Unity Platform. We're using the basic socketing library that is provided by microsoft.



How to use:

+ Our server is stored in 168WerewolfServer, You can run either from command line (assuming windows) or open our visual studio project. 

+ Our client is stored in WereWolf; our entry point scene is LoginScreen. 

+ Additionally, we've compiled the most recent build under "Submissions", where you should be able to access both client and server.


List of features:

- Sessions. You can specify a new session or join an existing one. The server will keep track and do print statements so you can check. If a server has to be created, it is made when a player logs in.

- Score board. Currently displays length of time spent online for each user, can be accessed by hitting <tab>

- Client d/c handling; clients will leave the game world and will also delete from score board.

- User login: users will be prompted for username and password; new users are made by specifying a non-existing username and password. Database access happens during login; only successful match will allow players to proceed. (or new account)



List of things to fix:

- Player spawning. (Incorrect number of people being spawned, but correct amount being tracked.)