using OrderGeneratorApp;
using QuickFix;
using QuickFix.Fields;

namespace OrderGeneratorTestApp;

[TestFixture]
public class OrderGeneratorTest
{

    OrderGeneratorService _orderGen = new OrderGeneratorService();
    
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void CheckMessage()
    {
        QuickFix.FIX44.NewOrderSingle message = _orderGen.CreateMessage(); 
        Assert.That(message, Is.TypeOf<QuickFix.FIX44.NewOrderSingle>());
    }

    [Test]
    public void CheckUuid()
    {
        ClOrdID uuid = _orderGen.QueryUuid(); 
        Assert.That(uuid, Is.TypeOf<ClOrdID>());
        Assert.NotNull(uuid);
    }

    [Test]
    public void QuerySymbol()
    {
        Symbol symbol = _orderGen.QuerySymbol(); 
        Assert.That(symbol, Is.TypeOf<Symbol>());
        Assert.NotNull(symbol);
        CollectionAssert.Contains(new [] {"PETR4", "VIIA4", "VALE3"}, symbol.ToString());
    }

    [Test]
    public void QuerySide()
    {
        Side side = _orderGen.QuerySide(); 
        Assert.That(side, Is.TypeOf<Side>());
        Assert.NotNull(side);
        CollectionAssert.Contains(new [] {"0", "1"}, side.ToString());
    }

    [Test]
    public void QueryOrderQty()
    {
        OrderQty qty = _orderGen.QueryOrderQty(); 
        Assert.That(qty, Is.TypeOf<OrderQty>());
        Assert.NotNull(qty);
        Assert.That(qty.getValue(), Is.InRange(1, 100000));
    }

    [Test]
    public void QueryPrice()
    {
        Price pr = _orderGen.QueryPrice(); 
        Assert.That(pr, Is.TypeOf<Price>());
        Assert.NotNull(pr);
        Assert.That(pr.getValue(), Is.InRange(1, 1000));
    }

    [Test]
    public void QueryTimeInForce()
    {
        TimeInForce pr = _orderGen.QueryTimeInForce(); 
        Assert.That(pr, Is.TypeOf<TimeInForce>());
        Assert.NotNull(pr);
    }

    
}