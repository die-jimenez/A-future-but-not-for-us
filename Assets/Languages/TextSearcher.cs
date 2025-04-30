using Game.Languages;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;



namespace Game.Languages
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class TextSearcher : MonoBehaviour
    {
        [SerializeField] LanguageContainer languageContainer;
        [SerializeField] GameManager.State screen;
        [SerializeField] string textID;
        [SerializeField] bool skipStarterSearch;

        [Header("Objeto buscado")]
        [SerializeField] LanguageData languageData;

        //Privadas
        TextMeshProUGUI visibleText;




        void Start()
        {
            visibleText = GetComponent<TextMeshProUGUI>();
            if(!skipStarterSearch)
            {
                languageData = SearchData(textID);
                SetText();
            }
            //Metodo global para cambiar idioma
            GameManager.instance.ApplyChangeLanguage.AddListener(SetText);
        }


        LanguageData SearchData(string _id)
        {
            if (_id == "") return null;

            switch (screen)
            {
                case GameManager.State.Menu:
                    return languageContainer.Content.Menu.Find(x => x.Id == _id);
                case GameManager.State.Game2D:
                    return languageContainer.Content.game2D.Find(x => x.Id == _id);
                case GameManager.State.Game3D:
                    return languageContainer.Content.game3D.Find(x => x.Id == _id);
            }
            return null;
        }


        void SetText()
        {
            if (languageData == null || languageData.Id == "")
            {
                Debug.Log(gameObject.name + " tiene el ID del TextSearch en blanco o sin el Language conatiner cargado");
                return;
            }

            if (GameManager.instance.language == GameManager.Language.Es)
            {
                visibleText.text = languageData.Es;
            }
            else if (GameManager.instance.language == GameManager.Language.En)
            {
                visibleText.text = languageData.En;
            }
        }

        string GetText()
        {
            //Debug.Log("GET TEXT SE EJECUTO: " + languageData);
            if (languageData == null || languageData.Id == "")
            {
                Debug.Log(gameObject.name + " tiene el ID del TextSearch en blanco o sin el Language conatiner cargado");
                return "Text error #01";
            }

            if (GameManager.instance.language == GameManager.Language.Es)
            {
                return languageData.Es;
            }
            else if (GameManager.instance.language == GameManager.Language.En)
            {
                return languageData.En;
            }
            else return "Text was not found";
        }


        public void UpdateText(string newId)
        {
            textID = newId;
            languageData = SearchData(textID);
            SetText();
        }

        public string GetUpdatedText(string newId)
        {
            textID = newId;
            languageData = SearchData(textID);
            return GetText();
        }

    }
}
