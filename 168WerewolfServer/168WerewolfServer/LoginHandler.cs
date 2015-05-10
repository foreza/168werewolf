using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Collections;


public class LoginHandler {
    dbAccess db = new dbAccess(); //database object
    public string DBName = "Database.db"; //name of database
    public string TableName = "LOGINS"; //name of login table
   // public UnityEngine.UI.InputField un; //username from input field
   // public UnityEngine.UI.InputField pw; //password from input field

	public string un;
	public string pw;

    public void StartDatabase() {
        db.OpenDB(TableName); //open table
        ArrayList columnNames = new ArrayList();
        columnNames.Add("usernames"); columnNames.Add("passwords");
        ArrayList columnValues = new ArrayList();
        columnValues.Add("string"); columnValues.Add("string");
        db.CreateTable(TableName, columnNames, columnValues);
    }

    public void CloseDatabase() {
        db.CloseDB();
    }

    void AddPair(string username, string password) {
		Console.WriteLine ("Adding pair: " + username + " | " + password);
        ArrayList col = new ArrayList();
        col.Add("usernames"); col.Add("passwords");
        ArrayList val = new ArrayList();
        val.Add(username); 
		val.Add(password);
        db.InsertIntoSpecific(TableName, col, val);
    }

    bool CheckIfExists(string username) {
        Console.WriteLine("Checking if username exists: " + username);
        ArrayList results = db.SingleSelectWhere(TableName, "usernames", "usernames", "=", username);
        return !(results.Count == 0);
    }

    bool CheckIfMatch(string username, string password) {
        Console.WriteLine("Checking if username: " + username + " | " + password);
        ArrayList results = db.BasicQuery("SELECT * FROM " + TableName + " WHERE usernames='" + username + "' AND passwords='" + password + "'");
        return !(results.Count == 0);
    }

    string correctLogin(bool newUser) {
        if (newUser) {
            Console.WriteLine("Welcome new user!");
            return "<login>:accept:new";
        }
        else {
            Console.WriteLine("Welcome back, friend!");
            return "<login>:accept:existing";
        }
    }

    string incorrectLogin() {
        Console.WriteLine("Incorrect login! Rejected!");
        return "<login>:reject";
    }

	public string AccessDB(string [] log){
        Console.WriteLine("Access DB with: " + log[0] + " | " + log[1]);
        string response;
		if (CheckIfExists(log[0])) {
			if (CheckIfMatch(log[0], log[1])) {
				response = correctLogin(false);
			}
			else {
				response = incorrectLogin();

			}
		}
		else {
			AddPair(log[0], log[1]);
			response = correctLogin(true);

		}
        return response;
		
		
	}
	
}
