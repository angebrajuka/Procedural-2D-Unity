using UnityEngine;
using TMPro;

public class MenuSPButtons : MonoBehaviour {
    void OnEnable() {
        int num = transform.childCount;
        for(int i=0; i<num; ++i) {
            var buttonTransform = transform.GetChild(i);
            var text = "new game";
            if(Save_Load.GetSaveInfo(i, out var info)) {
                text = info.ToString();
            }
            buttonTransform.GetChild(0).GetComponent<TextMeshProUGUI>().text = text;
        }
    }
}