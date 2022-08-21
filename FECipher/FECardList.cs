using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FECipher
{
    /// <summary>
    /// A Singleton Class Instance used by the Library to Read Card Data.
    /// </summary>
    internal class FECardList
    {
        internal static FECardList? instance;
        public FECard[] cardList;

        private FECardList()
        {
            cardList = new FECard[0];
        }

        private FECardList(List<FECard> feCardList)
        {
            // Order Array by ID
            cardList = feCardList.OrderBy(item => item.ID).ToArray();
        }

        public static FECardList Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new FECardList();
                }
                return instance;
            }
        }

        public static FECardList SetCardlist(List<FECard> feCardList)
        {
            if (instance == null)
            {
                instance = new FECardList(feCardList);
                return instance;
            }

            instance.cardList = feCardList.OrderBy(item => item.ID).ToArray();
            return instance;
        }

        // Used for Short-hand to access Card List
        public static FECard[] CardList
        {
            get
            {
                return Instance.cardList;
            }
        }

        public FECard GetCard(string cardId)
        {
            // Perform Binary Search
            int minNum = 0;
            int maxNum = this.cardList.Length - 1;

            while (minNum <= maxNum)
            {
                int mid = (minNum + maxNum) / 2;
                if (cardId == this.cardList[mid].ID)
                {
                    return this.cardList[mid];
                }
                else if (cardId.CompareTo(this.cardList[mid].ID) < 0)
                {
                    maxNum = mid - 1;
                }
                else
                {
                    minNum = mid + 1;
                }
            }

            throw new System.Data.DataException("Card cannot be found in cardlist.");
        }
    }
}
