
    public record GainItemData
    {
        public ItemType ItemType;
        public int ItemId;
        public double ItemValue;

        public GainItemData(ItemType itemType, int itemId, double itemValue)
        {
            ItemType = itemType;
            ItemId = itemId;
            ItemValue = itemValue;
        }
    }
