using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Test
{
    class Program
    {
        static async Task<int> AddsOneHundred(int n)
        {
            await Task.Delay(n * 1000);
            return n + 100;
        } 
        
        static async Task WriteAddsOneHundred(int n)
        {
            await Task.Delay(1000);
            Console.WriteLine(n + 100);
        } 
        
        static async Task Main(string[] args)
        {
            var watch = new Stopwatch();
            watch.Start();
            var calculations = new List<Func<int,Task<int>>>
            {
                AddsOneHundred,
                AddsOneHundred,
                AddsOneHundred
            };
            Console.WriteLine(watch.Elapsed);
            await Task.Delay(4000);
            Console.WriteLine(watch.Elapsed);
            
            // calculations[2]
            await calculations[2](2);
            // Thread pool -> new thread
            // main -> free
            // state machine -> {starting Thread; where to put result (where is the await);}
            // new thread -> is working on this
            Console.WriteLine(watch.Elapsed);
            Console.WriteLine(calculations[2]);
            Console.WriteLine(watch.Elapsed);
        }
    }
}