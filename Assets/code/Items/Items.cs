using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum ITEMTYPE{FOOD1=0,MEDICINE=1,MOUSE=2,FURNITURE=3}
public enum ILOC{VOID, HOUSE, BEACH, FOREST, INVENTORY, HAND, STORAGE}
public class Items : MonoBehaviour {

    static Items _items;
    public static Items items{get{if(!_items)_items = FindObjectOfType<Items>(); return _items;}}
    
    public static Item GetItem(int indx){
        return items.itemsList[indx];
    }

    public Sprite[] icons;

    public Sprite GetIcon(int item){
        return icons[(int)GetItem(item).iType];
    }
    List<Item> itemsList=null;
    public bool destroyOnLoad;
    public ItemInstance[] preset;
    void Awake(){
        if(itemsList==null){
            itemsList = new List<Item>();
            if(!destroyOnLoad)
                DontDestroyOnLoad(items);
        }  
        
    }

    public static void RemoveItem(int indx){

    }

    public void CreateItem(ItemPreset p, Vector3 startPos, bool startVisible){ if(itemsList==null){
            itemsList = new List<Item>();
            if(!destroyOnLoad)
                DontDestroyOnLoad(items);
        }  
        itemsList.Add(new Item(p,itemsList.Count, startVisible,startPos));
    }

    
    public void CreateItem(ItemBody b){ if(itemsList==null){
            itemsList = new List<Item>();
            if(!destroyOnLoad)
                DontDestroyOnLoad(items);
        }  
        itemsList.Add(new Item(b, itemsList.Count));
    }

    public void CreateItem(ItemBody b, int mouseIndex){
        itemsList.Add(new Item(b, itemsList.Count));
    }

    public class Item{
        
        public XTYPE mouseAction;
        public ItemBody body;
        int index;
        bool visible;
        Vector3 position{get{return body ? body.tform.position : Vector3.zero;}}
        public ITEMTYPE iType;
        ILOC _iLoc;
        public ILOC iLoc {get{return _iLoc;}}
        public bool storable;

        int _mouseIndex=-1;
        public int mouseIndex {get{return -1;}}

        public void SetMouseIndex(int _mouseIndex){
            this._mouseIndex=_mouseIndex;
        }
        public Item(ItemPreset itemPreset, int index, bool startVisible, Vector3 position){
            ItemBody b = Instantiate(itemPreset.bodyPrefab, position, Quaternion.Euler(itemPreset.baseRotation)).GetComponent<ItemBody>();
            body=b;
            body.index = index;
            body.SetVisible(startVisible);
            iType = itemPreset.type;
            _iLoc = ILOC.VOID;
        }

        public void SetVisible(bool viz){
            body.SetVisible(viz);
            visible = viz;
        }

        public Item(ItemBody body, int index){
            this.body = body;
            body.index=index;
            iType = body.type;
            _iLoc = ILOC.VOID;
        }

        public void SetStored(){
            body.SetVisible(false);
            visible=false;
        }
        public void SetLocation(ILOC location){
            _iLoc = location;
        }

        public void OnInteract(ProtagController protag, CONTROL ctl,Interactive target){

        }

        void XAction(ProtagController protag, Interactive target){

        }

        void YAction(ProtagController protag, Interactive target){

        }


    }


    [System.Serializable]
    public class ItemInstance{
        public ItemPreset preset;
        public Vector3 position;
        public bool startVisible;
    }

}