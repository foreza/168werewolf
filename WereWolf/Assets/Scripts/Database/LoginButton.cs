using UnityEngine;
using System.Collections;


public class LoginButton : MonoBehaviour {
    dbAccess db = new dbAccess();
    public string DBName = "TEST.db";
    public string TableName = "LOGINS";
    public UnityEngine.UI.InputField un;
    public UnityEngine.UI.InputField pw;
    public UnityEngine.UI.Text title;
    

    void Awake() {
        db.OpenDB(TableName);
        ArrayList columnNames = new ArrayList();
        columnNames.Add("usernames"); columnNames.Add("passwords");
        ArrayList columnValues = new ArrayList();
        columnValues.Add("string"); columnValues.Add("string");
        db.CreateTable(TableName, columnNames, columnValues);
    }

    void OnApplicationExit() {
        db.CloseDB();
    }

    void AddPair(string username, string password) {
        ArrayList col = new ArrayList();
        col.Add("usernames"); col.Add("passwords");
        ArrayList val = new ArrayList();
        val.Add(username); val.Add(password);
        db.InsertIntoSpecific(TableName, col, val);
    }

    bool CheckIfExists(string username) {
        ArrayList results = db.SingleSelectWhere(TableName, "usernames", "usernames", "=", username);
        return !(results.Count == 0);
    }

    bool CheckIfMatch(string username, string password) {
        ArrayList results = db.BasicQuery("SELECT * FROM " + TableName + " WHERE usernames='" + username + "' AND passwords='" + password + "'");
        return !(results.Count == 0);
    }

    void correctLogin(bool newUser) {
        if (newUser) {
            title.text = "Welcome new user!";
            title.color = Color.blue;
        }
        else {
            title.text = "Welcome Back!";
            title.color = Color.green;
        }
    }

    void incorrectLogin() {
        title.text = "Incorrect Login!";
        title.color = Color.red;
    }

    public void OnClick(int placeholder){
        if (CheckIfExists(un.text)) {
            if (CheckIfMatch(un.text, pw.text)) {
                correctLogin(false);
            }
            else {
                incorrectLogin();
            }
        }
        else {
            AddPair(un.text, pw.text);
            correctLogin(true);
        }
    }
}
