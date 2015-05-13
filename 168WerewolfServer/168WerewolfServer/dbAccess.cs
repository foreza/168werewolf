using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Threading;
using System.Data;
using System.Data.SQLite;



public class dbAccess {

    private SQLiteConnection dbcon = new SQLiteConnection();
    private SQLiteCommand dbcmd;
    private SQLiteDataReader reader;


    //Creates a database by the name specified if it does not already exist.
    //If it does exist, it just opens it.
    public void OpenDB(string dbName) {
        //Sets up the connection object to connect to the specified database.
        dbcon.ConnectionString = "URI=file:" + dbName;
        //Opens the connection so that commands can be made.
        dbcon.Open();
    }

    public void CreateTable(string name, ArrayList namesOfColumns, ArrayList columnTypes) {
        //Creates the table within the database.
        string query = "create table if not exists " + name + "(" + namesOfColumns[0] + " " + columnTypes[0];
        for (var i = 1; i < namesOfColumns.Count; i++) {
            query += ", " + namesOfColumns[i] + " " + columnTypes[i];
        }
        query += ")";
        dbcmd = dbcon.CreateCommand(); // create empty command
        dbcmd.CommandText = query; // fill the command
        reader = dbcmd.ExecuteReader(); // execute command which returns a reader
    }

    //This is the function you use if you want to make a query that doesn't have a premade function below.
    //Just pass the whole query and it'll run it.
    public ArrayList BasicQuery(string query) {
        //Set up a command
        dbcmd = dbcon.CreateCommand();
        //Make the text of the command into the query we want to make
        dbcmd.CommandText = query;

        reader = dbcmd.ExecuteReader();
        ArrayList readArray = new ArrayList();
        while (reader.Read()) {
            string temp = reader.GetString(0);
            readArray.Add(temp); // Fill array with all matches
            string url = reader.GetString(1);
            readArray.Add(url); // Fill array with all matches
        }
        return readArray; // return matches
    }

    //Honestly not sure that this function works
    //but we're not using it, so whatever.
    //It's supposed to put everything in the table into a DataTable object
    public DataTable ReadFullTable(string TableName) {
        string query = "SELECT * FROM " + TableName;
        dbcmd.CommandText = query;
        reader = dbcmd.ExecuteReader();
        DataTable toReturn = reader.GetSchemaTable();
        return toReturn;
    }

    //Deletes everything in the specified table. Dangerous.
    public void DeleteTableContents(string TableName) {
        string query = "DELETE FROM " + TableName;
        dbcmd = dbcon.CreateCommand();
        dbcmd.CommandText = query;
        reader = dbcmd.ExecuteReader();
    }


    //Sticks one value into one column
    public void InsertIntoSingle(string tableName, string colName, string value) { // single insert 
        string query = "INSERT INTO " + tableName + "(" + colName + ") " + "VALUES ('" + value + "')";
        dbcmd = dbcon.CreateCommand(); // create empty command
        dbcmd.CommandText = query; // fill the command
        reader = dbcmd.ExecuteReader(); // execute command which returns a reader
    }

    //Insert multiple values into specific columns
    public void InsertIntoSpecific(string tableName, ArrayList col, ArrayList values) {
        string query = "INSERT INTO " + tableName + " (" + col[0];
        for (int i = 1; i < col.Count; i++) {
            query += "," + col[i];
        }
        query += ") VALUES ('" + values[0] + "'";
        for (int i = 1; i < values.Count; i++) {
            query += ",'" + values[i] + "'";
        }
        query += ");";
        dbcmd = dbcon.CreateCommand();
        dbcmd.CommandText = query;
        reader = dbcmd.ExecuteReader();
    }

    //Fills a row in the table with the values in the ArrayList
    public void InsertInto(string tableName, ArrayList values) {
        string query = "INSERT INTO " + tableName + " VALUES ('" + values[0] + "'";
        for (int i = 1; i < values.Count; i++) {
            query += ",'" + values[i] + "'";
        }
        query += ");";
        dbcmd = dbcon.CreateCommand();
        dbcmd.CommandText = query;
        reader = dbcmd.ExecuteReader();
    }

    // This function reads a single column
    //  wCol is the WHERE column, wPar is the operator you want to use to compare with, 
    //  and wValue is the value you want to compare against.
    //  Ex. - SingleSelectWhere("puppies", "breed", "earType", "=", "floppy")
    //  returns an array of matches from the command: SELECT breed FROM puppies WHERE earType = floppy;
    //function SingleSelectWhere(tableName : String, itemToSelect : String, wCol : String, wPar : String, wValue : String):Array { // Selects a single Item
    public ArrayList SingleSelectWhere(string tableName, string itemToSelect, string wCol, string wPar, string wValue) { // Selects a single Item
        string query = "SELECT " + itemToSelect + " FROM " + tableName + " WHERE " + wCol + wPar + "'" + wValue + "'";
        dbcmd = dbcon.CreateCommand();
        dbcmd.CommandText = query;
        reader = dbcmd.ExecuteReader();
        ArrayList readArray = new ArrayList();
        while (reader.Read()) {
            string temp = reader.GetString(0);
            readArray.Add(temp); // Fill array with all matches
        }
        return readArray; // return matches
    }

    public void CloseDB() {
        reader.Close(); // clean everything up
        reader = null;
        dbcmd.Dispose();
        dbcmd = null;
        dbcon.Close();
        dbcon = null;
    }
}
