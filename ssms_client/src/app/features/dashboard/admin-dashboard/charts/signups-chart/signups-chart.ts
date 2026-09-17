import { Component, Input, OnChanges, ElementRef, ViewChild, AfterViewInit } from '@angular/core';
import { Chart, registerables } from 'chart.js';

Chart.register(...registerables);

@Component({
  selector: 'app-signups-chart',
  standalone: true,
  templateUrl: './signups-chart.html',
  styleUrl: './signups-chart.scss'
})
export class SignupsChart implements AfterViewInit, OnChanges {
  @Input() labels: string[] = [];
  @Input() data: number[] = [];
  @ViewChild('canvasRef') canvasRef!: ElementRef<HTMLCanvasElement>;

  private chart?: Chart;

  ngAfterViewInit(): void {
    this.renderChart();
  }

  ngOnChanges(): void {
    if (this.chart) this.renderChart();
  }

  private renderChart(): void {
    if (this.chart) this.chart.destroy();

    const primaryColor = getComputedStyle(document.documentElement).getPropertyValue('--color-primary').trim() || '#2f6fed';

    this.chart = new Chart(this.canvasRef.nativeElement, {
      type: 'line',
      data: {
        labels: this.labels,
        datasets: [{
          data: this.data,
          label: 'New Signups',
          borderColor: primaryColor,
          backgroundColor: 'transparent',
          tension: 0.3
        }]
      },
      options: {
        responsive: true,
        plugins: { legend: { display: false } }
      }
    });
  }
}