using System;
using System.Collections.Generic;
using System.Linq;

namespace backend.Services.Reports
{
    public interface IReportStrategyFactory
    {
        IReportStrategy GetStrategy(string reportType);
    }

    public class ReportStrategyFactory : IReportStrategyFactory
    {
        private readonly IEnumerable<IReportStrategy> _strategies;

        public ReportStrategyFactory(IEnumerable<IReportStrategy> strategies)
        {
            _strategies = strategies;
        }

        public IReportStrategy GetStrategy(string reportType)
        {
            var strategy = _strategies.FirstOrDefault(s => s.ReportType.Equals(reportType, StringComparison.OrdinalIgnoreCase));
            if (strategy == null)
            {
                throw new ArgumentException($"No report strategy found for type: {reportType}");
            }
            return strategy;
        }
    }
}
