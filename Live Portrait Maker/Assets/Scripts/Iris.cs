using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Iris : MonoBehaviour
{

    public DressManager dM;

    bool left;
    bool right;

private void Start() {
    left=true;
    right=true; 
}
    

    public void changeLeft()
    {
        Image leftI = transform.GetChild(5).GetComponent<Image>();
        dM.cpa.clearUpdateColor();
        if (left)
        {
            leftI.color = new Color(0.49f, 0.5f, 0.5f, 0.55f);
            
            if (right)
            {
                dM.cpa.UpdateColorAction += () =>
    {
        dM.fm.rightE[1].color = dM.cpa.Color;
    };
            }

        }
        else
        {
            leftI.color = Color.black;
			
			
                dM.cpa.UpdateColorAction += () =>
    {
        dM.fm.leftE[1].color = dM.cpa.Color;
		if (right)
            {
				dM.fm.rightE[1].color = dM.cpa.Color;
			}
    };
         
        }
        left = !left;
    }


	
    public void changeRight()
    {
        Image rightI = transform.GetChild(6).GetComponent<Image>();
        dM.cpa.clearUpdateColor();
        if (right)
        {
            rightI.color = new Color(0.49f, 0.5f, 0.5f, 0.55f);
            
            if (left)
            {
                dM.cpa.UpdateColorAction += () =>
    {
        dM.fm.leftE[1].color = dM.cpa.Color;
    };
            }

        }
        else
        {
            rightI.color = Color.black;
			
			
                dM.cpa.UpdateColorAction += () =>
    {
        dM.fm.rightE[1].color = dM.cpa.Color;
		if (left)
            {
				dM.fm.leftE[1].color = dM.cpa.Color;
			}
    };
         
        }
        right=!right;
    }
}
