using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _168WerewolfServer {
    class Scorekeeper {
        dbAccess db = new dbAccess();
        string tableName = "SCORES";
        void StartScoreTable(){
        db.OpenDB(tableName);
        ArrayList columnNames = new ArrayList();
        columnNames.Add("playerID"); columnNames.Add("score");
        ArrayList columnValues = new ArrayList();
        columnValues.Add("int"); columnValues.Add("string");
        db.CreateTable(tableName, columnNames, columnValues);
        }

        void SetScore(int playerID, int newScore) {
            db.BasicQuery("UPDATE "+tableName+" SET score = '"+newScore+"' WHERE playerID = '"+playerID+"'");
        }

        void ChangeScore(int playerID, int scoreDelta) {
            int newScore = 0;//currentScore+=scoreDelta;
            db.BasicQuery("UPDATE "+tableName+" SET score = '"+newScore+"' WHERE playerID = '"+playerID+"'");
        }

        void ResetScore(int playerID) {
            db.BasicQuery("UPDATE "+tableName+" SET score = 0 WHERE playerID = '"+playerID+"'");
        }

    }
}
