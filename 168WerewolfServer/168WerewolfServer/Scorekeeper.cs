using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _168WerewolfServer {
    public class Scorekeeper {
        dbAccess db = new dbAccess();
        string tableName;
        ArrayList columnNames = new ArrayList();
        ArrayList columnTypes = new ArrayList();

        //Right now things are set up to accomidate scores that update constantly throughout gameplay.
        //I don't think this is best for our game, but I'm doing it for testing purposes for now.
        //We should probably change it so that it's persistent across games.
        public Scorekeeper(string tName) {
            tableName = tName;
            db.OpenDB(tableName);
            columnNames.Add("username"); columnNames.Add("score");
            columnTypes.Add("string"); columnTypes.Add("int");
            db.CreateTable(tableName, columnNames, columnTypes);
        }

        //Deletes table when garbage collected
        //public ~Scorekeeper() {
        //    db.BasicQuery("DROP TABLE "+tableName);
        //}

        //Sets a player's score to a specified number
        public void SetScore(string username, int newScore) {
            ArrayList results = db.SingleSelectWhere(tableName, "username", "username", "=", username.ToString());
            if (!(results.Count == 0)) {
                db.BasicQuery("UPDATE " + tableName + " SET score = '" + newScore + "' WHERE username = '" + username + "'");
            }
            else {
                ArrayList temp = new ArrayList();
                temp.Add(username); temp.Add(newScore);
                db.InsertIntoSpecific(tableName, columnNames, temp);
            }
        }

        //Returns the specified player's score
        public int GetScore(string username) {
            ArrayList result = db.SingleSelectWhere(tableName, "score", "username", "=", username.ToString());
            return int.Parse((string)result[0]);
        }

        //Changes the specified player's score by the specified amount
        public void ChangeScore(string username, int scoreDelta) {
            ArrayList result = db.SingleSelectWhere(tableName, "score", "username", "=", username.ToString());
            int newScore = int.Parse((string)result[0]) + scoreDelta;
            db.BasicQuery("UPDATE " + tableName + " SET score = '" + newScore + "' WHERE username = '" + username + "'");
        }

        //Resets a specified player's score to 0
        public void ResetScore(string username) {
            db.BasicQuery("UPDATE " + tableName + " SET score = '0' WHERE username = '" + username + "'");
        }

        //Resets everyone's score to 0
        public void ResetAllScores() {
            ArrayList players = db.SingleSelect(tableName, "username");
            foreach (int playerID in players) {
                db.BasicQuery("UPDATE " + tableName + " SET score = '0' WHERE username = '" + playerID + "'");
            }
        }


    }
}
