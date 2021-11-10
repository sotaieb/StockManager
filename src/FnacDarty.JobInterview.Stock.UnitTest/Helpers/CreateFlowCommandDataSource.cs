using FnacDarty.JobInterview.Stock.Core.Commands;
using FnacDarty.JobInterview.Stock.Core.Entities;
using System.Collections;
using System.Collections.Generic;

namespace FnacDarty.JobInterview.Stock.UnitTest
{
    public class CreateFlowCommandDataSource
    {
        public static IEnumerable InvalidFlows
        {
            get
            {
                yield return null;
                yield return new CreateFlowCommand
                {

                };
                yield return new CreateFlowCommand
                {
                    Date = default,
                    Label = ""
                };
                yield return new CreateFlowCommand
                {
                    Date = default,
                    Label = "ABC",
                    FlowLines = null

                };

                yield return new CreateFlowCommand
                {
                    Date = default,
                    Label = "ABC",
                    FlowLines = new List<ProductLine>()

                };
                yield return new CreateFlowCommand
                {
                    Date = default,
                    Label = "ABC",
                    FlowLines = new List<ProductLine> { new ProductLine { } },

                };


                yield return new CreateFlowCommand
                {
                    Date = default,
                    Label = "ABC",
                    FlowLines = new List<ProductLine> { new ProductLine { Product = new Product { } } },

                };
                yield return new CreateFlowCommand
                {
                    Date = default,
                    Label = "ABC",
                    FlowLines = new List<ProductLine> { new ProductLine { Product = new Product {
                        Ean = "ABCD"
                    } } },

                };
                yield return new CreateFlowCommand
                {

                    Date = default,
                    Label = "ABC",
                    FlowLines = new List<ProductLine> { new ProductLine { Product = new Product {
                        Ean = "ABCDEFGHIJKLM"
                    } } },

                };
                yield return new CreateFlowCommand
                {
                    Date = default,
                    Label = "ABC",
                    FlowLines = new List<ProductLine> { new ProductLine { Product = new Product {
                        Ean = "ABCDEFGH" } ,
                        Quantity = 0

                    } },

                };
                yield return new CreateFlowCommand
                {
                    Date = default,
                    Label = "ABC",
                    IsRegularizationFlow = false,
                    FlowLines = new List<ProductLine> {
                        new ProductLine {
                                            Product = new Product {
                                                Ean = "ABCDEFGH"
                                        }
                                        , Quantity = -1

                        },
                         new ProductLine {
                                            Product = new Product {
                                            Ean = "ABCDEFGH"
                                        }
                                        , Quantity = -2

                        } }

                };
                yield return new CreateFlowCommand
                {

                    Date = default,
                    Label = "ABC",
                    IsRegularizationFlow = true,
                    FlowLines = new List<ProductLine> {
                        new ProductLine {
                                            Product = new Product {
                                                Ean = "ABCDEFGH"
                                        }
                                        , Quantity = -1

                        },
                         new ProductLine {
                                            Product = new Product {
                                            Ean = "ABCDEFGI"
                                        }
                                        , Quantity = 2

                        }
                    },

                };

            }
        }
    }
}
