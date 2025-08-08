export function countByStatus(
  items: any[],
  getStatus: (item: any) => string,
  statuses: string[]
): Record<string, number> {
  const counts: Record<string, number> = {};
  statuses.forEach((s) => (counts[s] = 0));
  items.forEach((item) => {
    const status = getStatus(item);
    if (counts[status] !== undefined) {
      counts[status]++;
    } else {
      counts['Unknown'] = (counts['Unknown'] || 0) + 1;
    }
  });
  return counts;
}

export function buildStatusChartOptions(
  counts: Record<string, number>,
  statuses: string[],
  charHeight: number
): Partial<any> {
  const isDark = document.documentElement.classList.contains('dark');
  return {
    theme: { mode: isDark ? 'dark' : 'light' },
    chart: {
      type: 'bar',
      height: charHeight,
      background: isDark ? '#374151' : '#fff',
      zoom: { enabled: false },
    },
    series: [
      {
        name: 'Auctions',
        data: statuses.map((s) => counts[s] || 0),
      },
    ],
    plotOptions: {
      bar: { horizontal: true, distributed: true, borderRadius: 4 },
    },
    dataLabels: {
      enabled: true,
      style: { colors: [isDark ? '#fff' : '#000'] },
    },
    xaxis: {
      categories: statuses,
      labels: {
        style: { colors: statuses.map(() => (isDark ? '#fff' : '#000')) },
      },
    },
    yaxis: {
      labels: { style: { colors: [isDark ? '#fff' : '#000'] } },
    },
  };
}

export function getBidsCountsByMonth(
  dataItems: any[],
  filterParams: any
): { labels: string[]; data: number[] } {
  const now = new Date();
  const months: string[] = [];
  const data: number[] = [];

  for (let i = 5; i >= 0; i--) {
    const monthDate = new Date(now.getFullYear(), now.getMonth() - i, 1);
    const label = monthDate.toLocaleString('default', {
      month: 'short',
      year: 'numeric',
    });
    months.push(label);

    const count = dataItems.filter((item: any) => {
      const bidTime = new Date(item[filterParams]);
      return (
        bidTime.getFullYear() === monthDate.getFullYear() &&
        bidTime.getMonth() === monthDate.getMonth()
      );
    }).length;
    data.push(count);
  }
  return { labels: months, data };
}

export function buildMonthlyChart(
  dataItems: any[],
  filter: any,
  type: 'bar' | 'line' | 'area' = 'bar',
  chartHeight: number
): Partial<any> {
  const { labels, data } = getBidsCountsByMonth(dataItems, filter);
  const isDark = document.documentElement.classList.contains('dark');

  return {
    chart: {
      type: type,
      height: chartHeight,
      background: isDark ? '#374151' : '#fff',
      zoom: { enabled: false },

    },
    theme: { mode: isDark ? 'dark' : 'light' },
    colors: [type === 'area' ? '#EF4444' : '#10B981'],
    series: [{ name: 'Creation', data }],

    plotOptions:
      type === 'bar'
        ? {
            bar: {
              borderRadius: 4,
              horizontal: true,
            },
          }
        : undefined,

    stroke:
      type !== 'bar'
        ? {
            curve: 'smooth',
            width: 2,
            colors: ['#36A2EB'],
          }
        : undefined,

    fill:
      type === 'area'
        ? {
            type: 'gradient',
            gradient: {
              shade: isDark ? 'dark' : 'light',
              opacityFrom: 0.4,
              opacityTo: 0.1,
              stops: [0, 100],
            },
          }
        : undefined,

    dataLabels: {
      enabled: true,
      style: {
        colors: [
          type === 'area'
            ? isDark
              ? 'red'
              : '#000'
            : isDark
            ? '#fff'
            : '#000',
        ],
      },
    },

    xaxis: {
      categories: labels,
      labels: {
        style: {
          colors: labels.map(() => (isDark ? '#fff' : '#000')),
        },
      },
      axisBorder: {
        color: isDark ? '#555' : '#ccc',
      },
      axisTicks: {
        color: isDark ? '#555' : '#ccc',
      },
    },

    yaxis: {
      labels: {
        style: {
          colors: [isDark ? '#fff' : '#000'],
        },
      },
    },

    grid: {
      borderColor: isDark ? '#444' : '#e0e0e0',
    },

    tooltip: {
      theme: isDark ? 'dark' : 'light',
    },
  };
}
