using System.Reflection;
var asm = Assembly.Load("PayOS");
var type = Type.GetType("PayOS.Models.V2.PaymentRequests.PaymentLinkStatus, PayOS")!;
Console.WriteLine($"TYPE: {type.FullName}");
foreach (var name in Enum.GetNames(type))
{
    Console.WriteLine(name);
}
