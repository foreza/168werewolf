using UnityEngine;
using System.Collections;

using System.Data;
using Mono.Data.SqliteClient;



public class dbAccess {

    private IDbConnection dbcon = new SqliteConnection();
    private IDbCommand dbcmd;
    private IDataReader reader;


    public void OpenDB(string dbName) { //Creates a connection with a database
        dbcon.ConnectionString = "URI=file:"+dbName;
        dbcon.Open();
    }

    public ArrayList BasicQuery(string query) { //Executes a basic query
        dbcmd = dbcon.CreateCommand();
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

    public DataTable ReadFullTable(string TableName) { //returns DataTable version of db
        string query = "SELECT * FROM " + TableName;
        dbcmd.CommandText = query;
        reader = dbcmd.ExecuteReader();
        DataTable toReturn = reader.GetSchemaTable();
		Debug.Log("ReadFullTable");
        return toReturn;
    }

    public void DeleteTableContents(string TableName) { //Deletes all data in table
        string query = "DELETE FROM " + TableName;
        dbcmd = dbcon.CreateCommand();
        dbcmd.CommandText = query;
        reader = dbcmd.ExecuteReader();
    }

    public void CreateTable(string name, ArrayList namesOfColumns, ArrayList columnTypes) {
        //A dictionary instead of two arraylists would probably be better, but I'd have to fix all
        //the functions to make them consistent and that's a pain in the ass
        string query = "create table if not exists " + name + "(" + namesOfColumns[0] + " " + columnTypes[0];
        for (var i = 1; i < namesOfColumns.Count; i++) {
            query += ", " + namesOfColumns[i] + " " + columnTypes[i];
        }
        query += ")";
        dbcmd = dbcon.CreateCommand(); // create empty command
        dbcmd.CommandText = query; // fill the command
        reader = dbcmd.ExecuteReader(); // execute command which returns a reader

	Debug.Log ("Table created!");
    }

    public void InsertIntoSingle(string tableName, string colName, string value) { // single insert 
        string query = "INSERT INTO " + tableName + "(" + colName + ") " + "VALUES ('" + value + "')";
        dbcmd = dbcon.CreateCommand(); // create empty command
        dbcmd.CommandText = query; // fill the command
        reader = dbcmd.ExecuteReader(); // execute command which returns a reader
		Debug.Log("InsertIntoSingle");
    }

    public void InsertIntoSpecific(string tableName, ArrayList col, ArrayList values) { // Specific insert with col and values
        string query = "INSERT INTO " + tableName + " (" + col[0];
        for(int i=1; i<col.Count; i++) {
            query += "," + col[i];
        }
        query += ") VALUES ('" + values[0]+"'";
        for(int i=1; i<values.Count; i++) {
            query += ",'" +values[i]+"'";
        }
        query += ");";
        dbcmd = dbcon.CreateCommand();
        dbcmd.CommandText = query; 
        reader = dbcmd.ExecuteReader();
		Debug.Log("InsertIntoSpecific");
    }

    public void InsertInto(string tableName, ArrayList values) { // basic Insert with just values
        string query = "INSERT INTO " + tableName + " VALUES ('" + values[0]+"'";
        for(int i=1; i<values.Count; i++) {
            query += ",'" + values[i]+"'";
        }
        query += ");";
        dbcmd = dbcon.CreateCommand();
        dbcmd.CommandText = query; 
        reader = dbcmd.ExecuteReader(); 
		Debug.Log("InsertInto");
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
        while(reader.Read()) { 
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
		Debug.Log("CloseDB");
    }
}
