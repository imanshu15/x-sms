# X-SMS
Stock Market simulation Game v0.1

This project was developed by group of students at UCD for the final year project. This project implements a Stock Market simulator.A player can create a game as public or private. If public anyone can join the game and if private only the players who have the gamecode can join the game.Maximum of 4 players and minimum of 2 players required to start a game.A Player AI could be added to the game if needed. The game has 10 rounds and each round last 1 minute. In start of each round the stock values gets changed according to the trends and events occured. Each player in a game gets an opening balance of 1000 and connected players can buy and sell stocks in each round.At the end of the round 10 the player who has invested well in stocks and gain the most profits will win the game.

## Getting Started

Download the solution from github. The solution consists of four projects.
  1. X-SMS : Web Application (ASP.Net MVC,JQuery,SignalR) - This contains the front end of the project
  2. X-SMS-API : API Project (Web API 2) - This contains the API controllers
  3. X-SMS-DAL : Data Access Layer (EntityFramework) - This contains the services and the database
  4. X-SMS-REP : Rpositories - This contains Data Transfer Objects
  
Open X-SMS.sln file in Visual Studio 2017.

### Prerequisites

  1. Microsoft Visual Studio 2017
  2. MSSQL 2017

### How to run

Once you opended the solution right click on the solution and click on Set StartUpProjects.In the popup select Multiple Projects and set X-SMS & X-SMS-API to Start then click OK. If you are using a local database change the "XSmsEntities" Connection string in the App.config file in X-SMS-DAL, if not you will be connected to our testing databse. Try and run the project.

### Troubleshoot

  1.Right click on the solution and click "Clean Solution"
  2.If packages are missing - Right click on the solution and click "Restore Nuget Packages"
  3.Run the project
  
## License

This project is licensed under the MIT License - see the [LICENSE.md](https://github.com/imanshu15/x-sms/blob/master/LICENSE) file for details

## Acknowledgments

* Hat tip to anyone whose code was used
* Inspiration
* etc
