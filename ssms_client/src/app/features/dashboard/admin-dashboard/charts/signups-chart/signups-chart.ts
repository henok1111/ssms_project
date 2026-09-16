import { Component, Input, OnChanges } from '@angular/core';
import { BaseChartDirective } from 'ng2-charts';
import { ChartConfiguration } from 'chart.js';

@Component({
  selector: 'app-signups-chart',
  standalone: true,
  imports: [BaseChartDirective],
  templateUrl: './signups-chart.html',
  styleUrl: './signups-chart.scss'
})
export class SignupsChart implements OnChanges {
  @Input() labels: string[] = [];
  @Input() data: number[] = [];

  chartData: ChartConfiguration<'line'>['data'] = { labels: [], datasets: [] };

  chartOptions: ChartConfiguration<'line'>['options'] = {
    responsive: true,
    plugins: { legend: { display: false } },
    scales: {
      x: { grid: { color: 'rgba(128,128,128,0.15)' } },
      y: { grid: { color: 'rgba(128,128,128,0.15)' }, beginAtZero: true }
    }
  };

  ngOnChanges(): void {
    this.chartData = {
      labels: this.labels,
      datasets: [{
        data: this.data,
        label: 'New Signups',
        borderColor: getComputedStyle(document.documentElement).getPropertyValue('--color-primary').trim() || '#2f6fed',
        backgroundColor: 'transparent',
        tension: 0.3
      }]
    };
  }
}