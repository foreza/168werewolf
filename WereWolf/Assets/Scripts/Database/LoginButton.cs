//using UnityEngine;
//using System.Collections;


//public class LoginButton : MonoBehaviour {
//    dbAccess db = new dbAccess(); //database object
//    public string DBName = "Database.db"; //name of database
//    public string TableName = "LOGINS"; //name of login table
//   // public UnityEngine.UI.InputField un; //username from input field
//   // public UnityEngine.UI.InputField pw; //password from input field
//    public UnityEngine.UI.Text title; //text title for editing

//    public string un;
//    public string pw;

//    public GameObject frowny;

    

//<<<<<<< HEAD
//    void Awake() {
//        db.OpenDB(TableName); //open table
//        ArrayList columnNames = new ArrayList();
//        columnNames.Add("usernames"); columnNames.Add("passwords");
//        ArrayList columnValues = new ArrayList();
//        columnValues.Add("string"); columnValues.Add("string");
//        db.CreateTable(TableName, columnNames, columnValues);
//    }

//    void OnApplicationExit() {
//        db.CloseDB();
//    }

//    void AddPair(string username, string password) {
//        print ("Adding pair: " + username + " | " + password);
//        ArrayList col = new ArrayList();
//        col.Add("usernames"); col.Add("passwords");
//        ArrayList val = new ArrayList();
//        val.Add(username); 
//        val.Add(password);
//        db.InsertIntoSpecific(TableName, col, val);
//    }

//    bool CheckIfExists(string username) {
//        print ("Checking if username exists: " + username);
//        ArrayList results = db.SingleSelectWhere(TableName, "usernames", "usernames", "=", username);
//        return !(results.Count == 0);
//    }

//    bool CheckIfMatch(string username, string password) {
//        print ("Checking if username: " + username + " | " + password);
//        ArrayList results = db.BasicQuery("SELECT * FROM " + TableName + " WHERE usernames='" + username + "' AND passwords='" + password + "'");
//        return !(results.Count == 0);
//    }

//    void correctLogin(bool newUser) {
//        if (newUser) {
//            //title.text = "Welcome new user!";
//           // title.color = Color.blue;
//            print ("Welcome new user!");
//            SendMessage("AcceptLogin");
//        }
//        else {
//           // title.text = "Welcome Back!";
//           // title.color = Color.green;
//            print ("Welcome back, friend!");
//            SendMessage("AcceptLogin");
//        }
//        Debug.Log("Load new scene");
//        Application.LoadLevel(Scenes.TITLE);
//    }

//    void incorrectLogin() {
//       // title.text = "Incorrect Login!";
//       // title.color = Color.red;
//        print ("DIE INSECT!");
//        SendMessage("RejectLogin");
//        frowny.SetActive(true);
//    }

//    public void AccessDB(string [] log){
//        print ("Access DB with: " + log [0] + " | " + log[1]);
//        if (CheckIfExists(log[0])) {
//            if (CheckIfMatch(log[0], log[1])) {
//                correctLogin(false);

//            }
//            else {
//                incorrectLogin();

//            }
//        }
//        else {
//            AddPair(log[0], log[1]);
//            correctLogin(true);

//        }
//=======
//    void Awake() {
//        db.OpenDB(TableName); //open table
//        ArrayList columnNames = new ArrayList();
//        columnNames.Add("usernames"); columnNames.Add("passwords");
//        ArrayList columnValues = new ArrayList();
//        columnValues.Add("string"); columnValues.Add("string");
//        db.CreateTable(TableName, columnNames, columnValues);
//    }

//    void OnApplicationExit() {
//        db.CloseDB();
//    }

//    void AddPair(string username, string password) {
//        print ("Adding pair: " + username + " | " + password);
//        ArrayList col = new ArrayList();
//        col.Add("usernames"); col.Add("passwords");
//        ArrayList val = new ArrayList();
//        val.Add(username); 
//        val.Add(password);
//        db.InsertIntoSpecific(TableName, col, val);
//    }

//    bool CheckIfExists(string username) {
//        print ("Checking if username exists: " + username);
//        ArrayList results = db.SingleSelectWhere(TableName, "usernames", "usernames", "=", username);
//        return !(results.Count == 0);
//    }

//    bool CheckIfMatch(string username, string password) {
//        print ("Checking if username: " + username + " | " + password);
//        ArrayList results = db.BasicQuery("SELECT * FROM " + TableName + " WHERE usernames='" + username + "' AND passwords='" + password + "'");
//        return !(results.Count == 0);
//    }

//    void correctLogin(bool newUser) {
//        if (newUser) {
//            //title.text = "Welcome new user!";
//           // title.color = Color.blue;
//            print ("Welcome new user!");
//            SendMessage("AcceptLogin");
//        }
//        else {
//           // title.text = "Welcome Back!";
//           // title.color = Color.green;
//            print ("Welcome back, friend!");
//            SendMessage("AcceptLogin");
//        }
//        Debug.Log("Load new scene");
//        //Application.LoadLevel(Scenes.TITLE);
//    }

//    void incorrectLogin() {
//       // title.text = "Incorrect Login!";
//       // title.color = Color.red;
//        print ("DIE INSECT!");
//        SendMessage("RejectLogin");
//        frowny.SetActive(true);
//    }

//    public void AccessDB(string [] log){
//        print ("Access DB with: " + log [0] + " | " + log[1]);
//        if (CheckIfExists(log[0])) {
//            if (CheckIfMatch(log[0], log[1])) {
//                correctLogin(false);

//            }
//            else {
//                incorrectLogin();

//            }
//        }
//        else {
//            AddPair(log[0], log[1]);
//            correctLogin(true);

//        }
//>>>>>>> 3bdd8fb03a884046f7644b5d19a5df3562ae7bc2
		
		
////    }
	
////}
