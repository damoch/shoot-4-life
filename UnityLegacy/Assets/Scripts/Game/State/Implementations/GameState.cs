using Assets.Scripts.Game.State.Abstracts;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Game.State.Implementations
{
    public class GameState : IGameState
    {
        public GameState()
        {
            TurnNumber = 1; //We start from 1 on this one
        }

        public int TurnNumber { get; set; }

        public string GetAboutString()
        {
            var sb = new StringBuilder();
            sb.Append(Application.productName);
            sb.Append(" by ");
            sb.Append(Application.companyName);
            sb.Append("\nVersion: ");
            sb.Append(Application.version);
            sb.Append("\nBUILD NO:");
            sb.Append(Application.buildGUID);

            return sb.ToString();
        }
    }
}

