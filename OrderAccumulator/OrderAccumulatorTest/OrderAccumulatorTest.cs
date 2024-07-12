using OrderAccumulatorApp;
using OrderAccumulatorApp.Exceptions;
using QuickFix;
using QuickFix.Fields;

namespace OrderAccumulatorTestApp;

[TestFixture]
public class OrderAccumulatorTest
{

    OrderAccumulatorService _orderAcc = new OrderAccumulatorService();
    QuickFix.FIX44.NewOrderSingle _buyOrder = new QuickFix.FIX44.NewOrderSingle(
                new ClOrdID("123456123"),
                new Symbol("PETR4"),
                new Side(Side.BUY),
                new TransactTime(DateTime.Now),
                new OrdType(OrdType.LIMIT));

    QuickFix.FIX44.NewOrderSingle _sellOrder = new QuickFix.FIX44.NewOrderSingle(
                    new ClOrdID("123456124"),
                    new Symbol("PETR4"),
                    new Side(Side.SELL),
                    new TransactTime(DateTime.Now),
                    new OrdType(OrdType.LIMIT));
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void CheckInitialSetup()
    {
        decimal expected = 0.0m;
        Assert.That(expected, Is.EqualTo(Accumulator.acc["PETR4"]));
        Assert.That(expected, Is.EqualTo(Accumulator.acc["VIIA4"]));
        Assert.That(expected, Is.EqualTo(Accumulator.acc["VALE3"]));
    }

    [Test]
    public void CheckLowerLimit()
    {
        _sellOrder.Set(new Price(10.0m));
        _sellOrder.Set(new HandlInst('1'));
        _sellOrder.Set(new OrderQty(100));
        _sellOrder.Set(new TimeInForce(TimeInForce.IMMEDIATE_OR_CANCEL));

        Assert.That(() => _orderAcc.CheckLimits(_sellOrder), Throws.Exception.TypeOf<AccumulatorLowerLimitException>());
    }

    [Test]
    public void CheckUpperLimitFail()
    {
        Accumulator.acc["PETR4"] = 99999000;
        _buyOrder.Set(new Price(10.0m));
        _buyOrder.Set(new HandlInst('1'));
        _buyOrder.Set(new OrderQty(1000));
        _buyOrder.Set(new TimeInForce(TimeInForce.IMMEDIATE_OR_CANCEL));

        Assert.That(() => _orderAcc.CheckLimits(_buyOrder), Throws.Exception.TypeOf<AccumulatorUpperLimitException>());
    }

    [Test]
    public void CheckUpperLimitPass()
    {
        _buyOrder.Symbol = new Symbol("VIIA4");
        _buyOrder.Set(new Price(10.0m));
        _buyOrder.Set(new HandlInst('1'));
        _buyOrder.Set(new OrderQty(1000));
        _buyOrder.Set(new TimeInForce(TimeInForce.IMMEDIATE_OR_CANCEL));

        try
        {
            _orderAcc.CheckLimits(_buyOrder);
        }
        catch (System.Exception e)
        {
            Assert.Fail("Expected no Exception but got: " + e.Message);            
        }
    }

}