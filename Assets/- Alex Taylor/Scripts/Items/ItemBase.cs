using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ATDungeon.Handlers;

namespace ATDungeon.Items
{
    public abstract class ItemBase : MonoBehaviour
    {
        public enum ItemState { Idle, Active, Cooldown};

        [Header("Item Settings")]
        [SerializeField]
        private float itemDuration;
        [SerializeField]
        private float effectInterval;
        [SerializeField]
        private float itemCoolDown;
        [SerializeField]
        private int numberOfUses;
        [SerializeField]
        private bool bInfiniteUses;

        [Header("Misc")]
        [SerializeField]
        private string itemName = "";
        [SerializeField]
        private int itemPurchaseValue = 0;
        [SerializeField]
        private int itemDiscardValue = 0;
        [SerializeField]
        private Sprite itemIcon;

        [Header("Debug")]
        [SerializeField]
        private ItemState currentState;

        private int currentNumberOfUses = 0;
        private float timeSinceLastUse = 0.0f;
        private float timeActive = 0.0f;
        private float intervalTime = 0.0f;
        private ItemHandler itemHandler = null;

        // Start is called before the first frame update
        void Start()
        {
            intervalTime = effectInterval;
            timeSinceLastUse = itemCoolDown;
            Initialise();
        }

        // Update is called once per frame
        void Update()
        {
            if(currentState == ItemState.Cooldown)
                ItemCoolDown();
            else if (currentState == ItemState.Active)
                ItemTimer();
        }

        private void ItemCoolDown()
        {
            timeSinceLastUse += Time.deltaTime;
            if (timeSinceLastUse >= itemCoolDown)
            {
                currentState = ItemState.Idle;
                itemHandler.UpdateItem(this);
            }
        }

        private void ItemTimer()
        {
            timeActive += Time.deltaTime;
            intervalTime -= Time.deltaTime;
            if (timeActive < itemDuration && intervalTime <= 0)
            {
                //Debug.Log("Item Action: " + this.gameObject.name);
                intervalTime = effectInterval;
                ItemAction();
            }
            else if (timeActive >= itemDuration)
            {
                intervalTime = effectInterval;
                currentState = ItemState.Cooldown;
                itemHandler.SetIsUsingItem(false);
                itemHandler.UpdateItem(this);
                timeActive = 0.0f;
                ItemComplete();
                if (currentNumberOfUses >= numberOfUses)
                    itemHandler.RemoveItem(this.gameObject);
            }
        }

        public void UseItem()
        {
            if (currentState == ItemState.Idle)
            {
                if (currentNumberOfUses < numberOfUses || bInfiniteUses)
                {
                    currentState = ItemState.Active;
                    itemHandler.SetIsUsingItem(true);
                    timeSinceLastUse = 0.0f;

                    itemHandler.UpdateItem(this);
                    ItemAction();
                    currentNumberOfUses++;
                }
                else
                {
                    itemHandler.RemoveItem(this.gameObject);
                }
            }
        }

        protected abstract void ItemAction();
        protected abstract void ItemComplete();
        protected abstract void Initialise();

        protected void UndoAction()
        {
            currentState = ItemState.Idle;
            timeSinceLastUse = itemCoolDown;
            itemHandler.UpdateItem(this);
            currentNumberOfUses--;
        }

        public string GetItemName() { return itemName; }
        public int GetItemDiscardValue() { return itemDiscardValue; }
        public int GetItemPurchaseValue() { return itemPurchaseValue; }
        public Sprite GetItemImage() { return itemIcon; }
        public int GetUsesRemaining() { return numberOfUses - currentNumberOfUses; }
        public int GetCurrentUses() { return currentNumberOfUses; }
        public void SetCurrentUses(int n) { currentNumberOfUses = n; }
        public void SetItemHandler(ItemHandler ih) { itemHandler = ih; }
        public void SetCurrentState(ItemState state) { currentState = state; }
        public ItemState GetCurrentState() { return currentState; }
    }
}
