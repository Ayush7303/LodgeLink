namespace LodgeLink.Areas.Residents.Services
{
    public interface ILateChargeCalculation
    {
        void CalculteLateCharge(int billid, int resid);
    }
}
