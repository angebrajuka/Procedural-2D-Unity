using UnityEngine;
using TMPro;

public class MenuSPButtons : MonoBehaviour {
    void OnEnable() {
        int num = transform.childCount;
        for(int i=0; i<num; ++i) {
            var buttonTransform = transform.GetChild(i);
            var text = "new game";
            var subText = "";
            if(Save_Load.GetSaveInfo(i, out var dateTime, out var saveName)) {
                text = saveName;
                subText = dateTime.ToString();
            }
            buttonTransform.GetChild(0).GetComponent<TextMeshProUGUI>().text = text;
            buttonTransform.GetChild(1).GetComponent<TextMeshProUGUI>().text = subText;
        }
    }
}