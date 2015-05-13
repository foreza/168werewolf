
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Assets.Scripts {
    struct PlayerUpdate {
        public int ID;

		public float xCoord;
		public float yCoord;

        public bool isWerewolf;
    }

    //Allows us to build up an update to send to the server so that
    //it's in a consistent format that the server can parse.
    class UpdateBuilder {
        PlayerUpdate pu = new PlayerUpdate();

        //Constructor requires ID of player who will send the update
        public UpdateBuilder(int initID) {
            pu.ID = initID;
        }
        
        public void setxCoord(float newXCoord) {
            pu.xCoord = newXCoord;
        }
        public void setyCoord(float newYCoord) {
            pu.yCoord = newYCoord;
        }

        public void setWerewolfState(bool iswolf) {
            pu.isWerewolf = iswolf;
        }



        public string BuildUpdate() {
            string update = JsonConvert.SerializeObject(pu);

            return update;
        }
    }
}

