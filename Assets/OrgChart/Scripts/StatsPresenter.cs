using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UniRx;

public class StatsPresenter : MonoBehaviour {

  [SerializeField] Text manPowerText;
  [SerializeField] Text moneyText;
  [SerializeField] Text yearText;
	// Use this for initialization
	void Start () {
    var gc = GameController.Instance;
    gc.manPower.SubscribeToText (manPowerText);
    gc.money.SubscribeToText (moneyText);
    gc.year
//      .Select(y => Util.AddOrdinal(y) + " year")
      .Select(y => y.ToString() + "年目")
      .SubscribeToText (yearText);
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
