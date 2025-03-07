using System.Text.Json.Serialization;

namespace BookBee.Model
{
	public enum DiscountType
	{
		
		Percentage=0,
		FixedAmount=1
	}

    public enum OrderTypeEnum
    {
        BanTaiQuay = 0,  // Bán tại quầy
        BanOnline = 1     // Bán online
    }
}
