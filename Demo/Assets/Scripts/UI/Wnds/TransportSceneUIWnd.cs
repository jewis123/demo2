using Battle;
using Battle.Config;
using Scene.SceneControllers;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Wnds
{
    public class TransportSceneUIWnd : UIController
    {

        [SerializeField] private RectTransform listItemParent;
        [SerializeField] private GameObject listItemPrefab;
        
        public override void OnOpen(object[] param)
        {
            if (param != null)
            {
                for (int i = 0; i < param.Length; i++)
                {
                    var item = GameObject.Instantiate(listItemPrefab, listItemParent, false);
                    item.name = param[i] as string;
                    item.GetComponent<Button>().onClick.AddListener(() =>
                    {
                        if (item.name == "BattleScene")
                        {
                            GameCore.singleton.gameSceneManager.UnLoadScene<ExampleScene>();

                            Close();

                            var config = Resources.Load<BattleDataConfig>("ConfigObjs/BattleData");
                            GameCore.singleton.battleManager.StartBattle(config);
                        }
                    });
                }
            }
        }
        

        public override void OnClose()
        {
        }
        
        
        public void OnCloseButtonClk()
        {
            Close();
        }
    }
}