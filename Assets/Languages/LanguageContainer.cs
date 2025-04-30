using NorskaLib.Spreadsheets;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Game.Languages
{
    [Serializable]
    public class SpreadshetContent
    {
        [SpreadsheetPage("Menu")]
        public List<LanguageData> Menu;
        [SpreadsheetPage("Game2D")]
        public List<LanguageData> game2D;
        [SpreadsheetPage("Game3D")]
        public List<LanguageData> game3D;
    }

    [CreateAssetMenu(fileName = "LanguageContainer", menuName = "SpreadsheetContainer")]
    public class LanguageContainer : SpreadsheetsContainerBase
    {
        [SpreadsheetContent]
        [SerializeField] SpreadshetContent content;
        public SpreadshetContent Content => content;
    }

}


