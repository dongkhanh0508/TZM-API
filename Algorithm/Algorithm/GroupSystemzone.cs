using System;
using System.Collections.Generic;
using System.Linq;
using TradeMap.Algorithm.Graph;
using TradeMap.Algorithm.Model;
using TradeMap.Data.Entity;

namespace TradeMap.Algorithm.Algorithm
{
    public class GroupSystemzone
    {
        public static TradeMap.Algorithm.Graph.Graph InitGraph(List<SystemZone> listSystemzone)
        {
            TradeMap.Algorithm.Graph.Graph g = new TradeMap.Algorithm.Graph.Graph();
            try
            {
                foreach (var item in listSystemzone)
                {
                    g.AddVertext(new Vertex(item));
                }
                foreach (var item in g)
                {
                    var listEdge = listSystemzone.Where(x => x.Id != item.SystemZone.Id).ToList();
                    foreach (var edge in listEdge)
                    {
                        if (item.SystemZone.Geom.Intersects(edge.Geom))
                        {
                            g.AddEdge(item.SystemZone, edge);
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return g;
        }

        public static List<StoreTradezone> GroupSystemzoneForStoresByDistance(double distance, List<SystemZone> listSystemzone, List<Store> StoreInBrand)
        {
            List<TradeZoneModel> model = new List<TradeZoneModel>();
            List<TradeZoneModel> tradeZoneModels = new List<TradeZoneModel>();
            var totalStore = StoreInBrand.Count;
            foreach (var item in StoreInBrand)
            {
                TradeZoneModel temp = new TradeZoneModel()
                {
                    SelectedSystemzone = listSystemzone.Where(x => x.Geom.Contains(item.Geom)).FirstOrDefault(),
                    ListSuitable = GetSystemzonesSuitable(item, listSystemzone, distance),
                    ListSystemZones = new List<SystemZone>(),
                    Store = item,
                };
                model.Add(temp);
            }
            var systemzones = model.Select(x => x.SelectedSystemzone).Distinct();

            if (systemzones.Count() < totalStore)
            {
                foreach (var systemzone in systemzones)
                {
                    var tempModel = model.Where(x => x.SelectedSystemzone == systemzone).FirstOrDefault();
                    tradeZoneModels.Add(tempModel);
                }
                var tempList = new List<TradeZoneModel>(tradeZoneModels);
                tradeZoneModels = model.Where(x => tempList.All(c => x.Store.Id != c.Store.Id)).ToList();
                model = (tempList);
            }

            totalStore = model.Count;
            List<SystemZone> listConflicts = new List<SystemZone>();

            model.ForEach(c => listConflicts.AddRange(c.ListSuitable));
            listConflicts = GetConflictSystemzone(listConflicts);
            foreach (var item in model)
            {
                item.ListSuitable = item.ListSuitable.Where(x => model.All(c => c.SelectedSystemzone.Id != x.Id)).ToList();
            }
            List<SystemZone> systemZones = new List<SystemZone>();
            model.ForEach(c => c.ListSystemZones = GetComplementSystemzone(listConflicts, c.ListSuitable));
            foreach (var item in model)
            {
                item.ListSystemZones.Add(item.SelectedSystemzone);
                systemZones.Add(item.SelectedSystemzone);
            }
            model = CalculateTotalWeight(model);

            model.ForEach(c => systemZones.AddRange(c.ListSuitable));
            systemZones.AddRange(listConflicts);
            model.ForEach(item => item.ListSuitable = item.ListSuitable.Where(x => item.ListSystemZones.All(c => c != x)).ToList());
            systemZones = systemZones.Distinct().ToList();
            var avgWeight = CalculateAvgWeight(systemZones, totalStore);
            int? standardDeviation = 0;
            foreach (var item in model)
            {
                var temp = CalculateStandardDeviation(avgWeight, item.TotalWeight);

                if (standardDeviation < temp)
                {
                    standardDeviation = temp;
                }
            }

            List<TradeZoneModel> list = new List<TradeZoneModel>(model);

            list = list.OrderBy(x => x.SelectedSystemzone.Id).ToList();
            model = AddSystemzoneFromListConflict(list, avgWeight, systemZones);

            var result = model.Select(c => new StoreTradezone
            {
                Store = c.Store,
                ListSystemzone = c.ListSystemZones,
                TotalWeight = c.TotalWeight
            }).ToList();
            foreach (var item in tradeZoneModels)
            {
                var duplicateZone = model.Where(x => x.SelectedSystemzone == item.SelectedSystemzone).FirstOrDefault();
                var storeTradezone = new StoreTradezone()
                {
                    Store = item.Store,
                    ListSystemzone = duplicateZone.ListSystemZones,
                    TotalWeight = duplicateZone.TotalWeight
                };
                result.Add(storeTradezone);
            }
            return result;
        }

        public static List<SystemZone> GetSystemzonesSuitable(Store Store, List<SystemZone> listSystemZones, double distance)
        {
            List<SystemZone> rs = new List<SystemZone>();
            rs = listSystemZones.Where(x => x.Geom.Coordinates.ToList().Max(c => c.Distance(Store.Geom.Coordinate)) * 40075.017 / 360 <= distance).ToList();
            return rs;
        }

        public static List<SystemZone> GetConflictSystemzone(List<SystemZone> list)
        {
            List<SystemZone> result = new List<SystemZone>();
            var groups = list.GroupBy(v => v).Where(x => x.Count() > 1);

            foreach (var group in groups)
                result.Add(group.Key);
            return result;
        }

        public static List<SystemZone> GetComplementSystemzone(List<SystemZone> listConflict, List<SystemZone> listCurrentSystemzone)
        {
            return listCurrentSystemzone.Where(a => !listConflict.Any(b => b.Id == a.Id)).ToList();
        }

        public static List<TradeZoneModel> CalculateTotalWeight(List<TradeZoneModel> list)
        {
            foreach (var item in list)
            {
                foreach (var systemzone in item.ListSystemZones)
                {
                    if (systemzone.WeightNumber < 0)
                    {
                        return null;
                    }
                    item.TotalWeight += systemzone.WeightNumber;
                }
            }
            return list;
        }

        public static int? CalculateStandardDeviation(double? avg, double? total)
        {
            int? rs;
            if (avg < 0 || total < 0)
            {
                return null;
            }
            if (total > avg)
            {
                var temp = total - avg;
                if (temp - (int)temp > 0)
                {
                    rs = (int)temp + 1;
                }
                else return (int)temp;
            }
            else
                rs = 1;
            return rs;
        }

        public static double? CalculateAvgWeight(List<SystemZone> sys, int totalStore)
        {
            double? rs = 0;
            sys = sys.Distinct().ToList();
            foreach (var item in sys)
            {
                if (item.WeightNumber < 0)
                {
                    return null;
                }
                rs += item.WeightNumber;
            }
            rs /= totalStore;
            return rs;
        }

        public static List<TradeZoneModel> AddSystemzoneFromListConflict(List<TradeZoneModel> list, double? avg, List<SystemZone> graph)
        {
            var g = InitGraph(graph);
            List<TradeZoneModel> tempList = list.Select(x => new TradeZoneModel
            {
                ListSuitable = new List<SystemZone>(x.ListSuitable),
                ListSystemZones = new List<SystemZone>(x.ListSystemZones),
                SelectedSystemzone = x.SelectedSystemzone,
                Store = x.Store,
                TotalWeight = x.TotalWeight
            }).ToList();

            foreach (var model in list)
            {
                var tempGraph2 = InitGraph(model.ListSystemZones);
                tempGraph2.DFS(model.SelectedSystemzone.Id);
                model.ListSystemZones = tempGraph2.Where(x => x.IsVisit).Select(x => x.SystemZone).ToList();
                double? weight = 0.0;
                foreach (var item in model.ListSystemZones)
                {
                    weight += item.WeightNumber;
                }
                model.TotalWeight = weight;
                model.ListSuitable.AddRange(tempGraph2.Where(x => !x.IsVisit).Select(x => x.SystemZone).ToList());
                model.ListSuitable.AddRange(model.ListSystemZones);
                Graph.Graph tempGraph = new Graph.Graph();
                tempGraph = InitGraph(model.ListSuitable);
                tempGraph.BFS(model.SelectedSystemzone.Id);
                List<Vertex> tempGraph1 = new List<Vertex>();
                tempGraph1 = tempGraph.OrderBy(x => x.Level).ToList();
                tempGraph1 = tempGraph1.Where(x => x.Level != -1).ToList();
                model.ListSuitable = tempGraph1.Select(x => x.SystemZone).ToList();

                model.ListSuitable = model.ListSuitable.Where(x => model.ListSystemZones.All(c => c != x)).ToList();

                foreach (var systemzone in model.ListSystemZones)
                {
                    g.GetVertex(systemzone.Id).IsCollect = true;
                    g.GetVertex(systemzone.Id).SelectedId = model.SelectedSystemzone.Id;
                }
            }
            foreach (var model in list)
            {
                double? standardDeviation = double.MaxValue;
                double? range = 0;
                foreach (var item in model.ListSuitable)
                {
                    if (model.ListSystemZones.Any(x => x.Geom.Intersects(item.Geom)) && g.GetVertex(item.Id).IsCollect == false)
                    {
                        var temp1 = model.TotalWeight + item.WeightNumber;
                        range = Math.Abs((double)(avg - temp1));
                        if (range < standardDeviation)
                        {
                            standardDeviation = range;
                            model.ListSystemZones.Add(item);
                            model.TotalWeight = temp1;
                            g.GetVertex(item.Id).IsCollect = true;
                            g.GetVertex(item.Id).SelectedId = model.SelectedSystemzone.Id;
                        }
                        if (range <= standardDeviation && (avg - temp1) < 0)
                        {
                            break;
                        }
                    }
                }
            }
            List<SystemZone> remainSystemzone = new List<SystemZone>();
            foreach (var item in g)
            {
                if (item.IsCollect == false) remainSystemzone.Add(item.SystemZone);
            }
            var deleteList = new List<SystemZone>();
            foreach (var item in remainSystemzone)
            {
                var selectSystemzone = list.Where(x => x.ListSystemZones.Any(c => c.Geom.Intersects(item.Geom)));
                if (selectSystemzone != null)
                    if (selectSystemzone.Count() == 1)
                    {
                        selectSystemzone.First().TotalWeight = selectSystemzone.First().TotalWeight + item.WeightNumber;
                        selectSystemzone.First().ListSystemZones.Add(item);
                        g.GetVertex(item.Id).SelectedId = selectSystemzone.First().SelectedSystemzone.Id;
                        deleteList.Add(item);
                    }
            }
            foreach (var item in deleteList)
            {
                remainSystemzone.Remove(item);
            }

            while (remainSystemzone.Any())
            {
                var deleteDuplicateList = new List<SystemZone>();
                foreach (var item in remainSystemzone)
                {
                    var selectSystemzone = list.Where(x => x.ListSystemZones.Any(c => c.Geom.Intersects(item.Geom))).OrderBy(x => x.TotalWeight).FirstOrDefault();
                    if (selectSystemzone != null)
                    {
                        selectSystemzone.TotalWeight += item.WeightNumber;
                        selectSystemzone.ListSystemZones.Add(item);
                        g.GetVertex(item.Id).SelectedId = selectSystemzone.SelectedSystemzone.Id;
                        deleteDuplicateList.Add(item);
                    }
                }
                foreach (var item in deleteDuplicateList)
                {
                    remainSystemzone.Remove(item);
                }
            }

            List<TradeZoneModel> listNeedToSplit = new List<TradeZoneModel>(list);
            var listSelected = list.Select(x => x.SelectedSystemzone.Id).ToList();
            while (listNeedToSplit.Any())
            {
                var minTradeZone = listNeedToSplit.OrderBy(x => x.TotalWeight).First();
                for (int i = 0; i < list.Where(x => x.SelectedSystemzone.Id == minTradeZone.SelectedSystemzone.Id).FirstOrDefault().ListSystemZones.Count; i++)
                {
                    var item = list.Where(x => x.SelectedSystemzone.Id == minTradeZone.SelectedSystemzone.Id).FirstOrDefault().ListSystemZones.ElementAt(i);
                    var templist = g.GetVertex(item.Id).AdjList.ToList();

                    foreach (var edge in templist)
                    {
                        double? tempWeght = list.Where(x => x.SelectedSystemzone.Id == minTradeZone.SelectedSystemzone.Id).FirstOrDefault().TotalWeight;
                        tempWeght += edge.V.SystemZone.WeightNumber;
                        if (tempWeght / avg <= 0.95)
                        {
                            var selectedId = g.GetVertex(edge.V.SystemZone.Id).SelectedId;

                            if (selectedId != 0)
                            {
                                List<SystemZone> listToCheckLink = new List<SystemZone>(list.Where(x => x.SelectedSystemzone.Id == selectedId).FirstOrDefault().ListSystemZones);
                                listToCheckLink.Remove(edge.V.SystemZone);
                                if (CheckLinkTradeZone(listToCheckLink) && listSelected.All(x => x != edge.V.SystemZone.Id))
                                {
                                    g.GetVertex(edge.V.SystemZone.Id).SelectedId = 0;
                                    list.Where(x => x.SelectedSystemzone.Id == minTradeZone.SelectedSystemzone.Id).FirstOrDefault().ListSystemZones.Add(edge.V.SystemZone);
                                    list.Where(x => x.SelectedSystemzone.Id == selectedId).FirstOrDefault().ListSystemZones.Remove(edge.V.SystemZone);
                                    list.Where(x => x.SelectedSystemzone.Id == minTradeZone.SelectedSystemzone.Id).FirstOrDefault().TotalWeight = tempWeght;
                                    list.Where(x => x.SelectedSystemzone.Id == selectedId).FirstOrDefault().TotalWeight -= edge.V.SystemZone.WeightNumber;
                                }
                            }
                        }
                    }
                }
                listNeedToSplit.Remove(minTradeZone);
            }
            return list;
        }

        public static bool CheckLinkTradeZone(List<SystemZone> lists)
        {
            var g = InitGraph(lists);
            g.DeepFirstSearch();
            var wardId = g.Select(x => x.Label).Distinct().ToList();
            var result = (wardId.Count == 1);
            return result;
        }
    }
}