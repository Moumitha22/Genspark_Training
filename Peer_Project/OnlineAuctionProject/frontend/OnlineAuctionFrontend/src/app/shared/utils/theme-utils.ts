import {
  buildMonthlyChart,
  buildStatusChartOptions,
  countByStatus,
} from './dashboard-utils';

export function loadTheme() {
  const theme = localStorage.getItem('theme');
  const root = document.documentElement;
  if (theme) {
    root.classList.add(theme);
  }
}

export function observeThemeChanges(
  auctionItems: any,
  filterationValue: any,
  getAuctionStatus: any,
  status: any,
  hasViewInitialized: boolean,
  statusChart: any,
  monthChart: any,
  statusChartOptions: any,
  monthlyChartOptions: any,
  chartHeight: number,
  onStatusChartOptionsChange: (opts: any) => void,
  onMonthlyCharOptionsChange: (opts: any) => void
): any {
  const observer = new MutationObserver(() => {
    if (status) {
      const counts = countByStatus(auctionItems, getAuctionStatus, status);
      refreshStatusChart(hasViewInitialized, statusChart, statusChartOptions);
      const newOptions = buildStatusChartOptions(counts, status, chartHeight);
      onStatusChartOptionsChange(newOptions);
    }
    const newOptionMonthly = buildMonthlyChart(
      auctionItems,
      filterationValue,
      undefined,
      chartHeight
    );
    refreshMonthlyCharts(hasViewInitialized, monthChart, monthlyChartOptions);
    onMonthlyCharOptionsChange(newOptionMonthly);
  });

  observer.observe(document.documentElement, {
    attributes: true,
    attributeFilter: ['class'],
  });
}

function refreshStatusChart(
  hasViewInitialized: boolean,
  statusChart: any,
  statusChartOptions: any
) {
  if (hasViewInitialized && statusChart && statusChartOptions) {
    statusChart.updateOptions(statusChartOptions as any, true, true);
  }
}

function refreshMonthlyCharts(
  hasViewInitialized: boolean,
  monthChart: any,
  monthChartOptions: any
) {
  if (
    hasViewInitialized &&
    monthChart &&
    typeof monthChart.updateOptions === 'function' &&
    monthChartOptions
  ) {
    monthChart.updateOptions(monthChartOptions as any, true, true);
  }
}
