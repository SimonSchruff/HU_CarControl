using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteInEditMode()]
public class ScriptableUI : MonoBehaviour
{
   public ScriptableUIData skinData; 

   public virtual void Awake()
   {
       OnSkinUI(); 
   }

   protected virtual void OnSkinUI()
   {

   }

   public virtual void Update()
   {
       //Allows changes in Editor
       //If performance issues, its better to build custom editor script with Update fct
       if(Application.isEditor)
       {
           OnSkinUI(); 
       }
            
   }

}
