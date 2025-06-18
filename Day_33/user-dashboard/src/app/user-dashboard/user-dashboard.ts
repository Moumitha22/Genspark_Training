import { Component, OnInit, inject } from '@angular/core';
import { UserModel } from '../models/user.model';
import { UserService } from '../services/user.service';
import { AddUser } from '../add-user/add-user';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { NgChartsModule } from 'ng2-charts';
import { ChartData, ChartOptions, Chart} from 'chart.js';
import ChartDataLabels from 'chartjs-plugin-datalabels';
import { Router } from '@angular/router';
Chart.register(ChartDataLabels);

@Component({
  selector: 'app-user-dashboard',
  imports: [FormsModule, NgChartsModule],
  templateUrl: './user-dashboard.html',
  styleUrl: './user-dashboard.css'
})
export class UserDashboard implements OnInit {
  private userService = inject(UserService);
  private router = inject(Router);

  users: UserModel[] = [];
  selectedGender:string = "";
  selectedRole:string = "";
  searchText:string = "";
  selectedState:string = "";

  genderChartType: 'pie' = 'pie';
  genderChartLabels: string[] = ['Male', 'Female'];
  genderChartData: ChartData<'pie', number[], string> = {
    labels: ['Male', 'Female'],
    datasets: [
      {
        data: [0, 0],
        backgroundColor: ['#36A2EB', '#FF6384'],
        hoverOffset: 6
      }
    ]
  };
  chartOptions: ChartOptions<'pie'> = {
    responsive: true,
    maintainAspectRatio: false,
    plugins: {
      legend: {
        position: 'bottom',
        labels: {
          font: {
            size: 12
          }
        }
      },
      tooltip: {
        callbacks: {
          label: function (context) {
            const label = context.label || '';
            const value = context.raw;
            return `${label}: ${value}`;
          }
        }
      },
      datalabels: {
        color: '#000',
        font: {
          weight: 'bold' as const,
          size: 10
        },
        formatter: (value, context) => {
          const dataset = context.chart.data.datasets[0].data as number[];
          const total = dataset.reduce((sum, val) => sum + val, 0);
          const percentage = total > 0 ? (value / total * 100).toFixed(1) : '0.0';
          return `${percentage}%`;
        }
      }
    }
  };

  roleChartType: 'bar' = 'bar';
  roleChartData: ChartData<'bar', number[], string> = {
    labels: [], 
    datasets: [
      {
        label: 'Users per Role',
        data: [],
        backgroundColor: ['#4e73df', '#1cc88a', '#36b9cc', '#f6c23e'],
        hoverBackgroundColor: ['#2e59d9', '#17a673', '#2c9faf', '#f4b619']
      }
    ]
  };
  roleChartOptions: ChartOptions<'bar'> = {
    responsive: true,
    maintainAspectRatio: false,
    plugins: {
      legend: {
        display: true,
        position: 'top',
      },
      tooltip: {
        callbacks: {
          label: function (context) {
            const label = context.dataset.label || '';
            return `${label}: ${context.raw}`;
          }
        }
      }
    },
    scales: {
      x: {
        title: {
          display: true,
          text: 'Role'
        }
      },
      y: {
        beginAtZero: true,
        title: {
          display: true,
          text: 'User Count'
        }
      }
    }
  };


  stateChartType: 'bar' = 'bar';
  stateChartData: ChartData<'bar', number[], string> = {
    labels: [],  
    datasets: [
      {
        label: 'Users per State',
        data: [], 
        backgroundColor: ['#36A2EB', '#FF6384', '#FFCE56', '#4BC0C0', '#9966FF', '#FF9F40'], // or generate dynamically
        hoverBackgroundColor: ['#2e59d9', '#e55373', '#e5b437', '#3aa7a7', '#875fd9', '#f48629']
      }
    ]
  };

  stateChartOptions: ChartOptions<'bar'> = {
    responsive: true,
    maintainAspectRatio: false,
    plugins: {
      legend: {
        display: false
      },
      tooltip: {
        callbacks: {
          label: function (context) {
            const label = context.dataset.label || '';
            return `${label}: ${context.raw}`;
          }
        }
      }
    },
    scales: {
      x: {
        title: {
          display: true,
          text: 'State'
        }
      },
      y: {
        beginAtZero: true,
        title: {
          display: true,
          text: 'User Count'
        }
      }
    }
  };

  ngOnInit(): void {
    this.userService.users$.subscribe({
      next: (data) => {
        this.users = data;
        this.updateChartData();
      },
      error: (err) => console.error('Error loading users:', err),
    });

  }
  get totalUsers(): number {
    return this.users.length;
  }

  get maleUsers(): number {
    return this.users.filter(u => u.gender === 'male').length;
  }

  get femaleUsers(): number {
    return this.users.filter(u => u.gender === 'female').length;
  }

  get adminUsers(): number {
    return this.users.filter(u => u.role === 'Admin').length;
  }

  updateChartData() {
    const filtered = this.filteredUsers;

    const males = filtered.filter(u => u.gender === 'male').length;
    const females = filtered.filter(u => u.gender === 'female').length;

    this.genderChartData = {
      labels: ['Male', 'Female'],
      datasets: [
        {
          data: [males, females],
          backgroundColor: ['#36A2EB', '#FF6384'],
          hoverOffset: 6
        }
      ]
    };

    const roleCounts: { [role: string]: number } = {};
    filtered.forEach(user => {
      roleCounts[user.role] = (roleCounts[user.role] || 0) + 1;
    });
      
    this.roleChartData = {
      labels: Object.keys(roleCounts),
      datasets: [
        {
          label: 'Users per Role',
          data: Object.values(roleCounts),
          backgroundColor: ['#4e73df', '#1cc88a', '#36b9cc', '#f6c23e'],
          hoverBackgroundColor: ['#2e59d9', '#17a673', '#2c9faf', '#f4b619']
        }
      ]
    };

    const stateCounts: { [state: string]: number } = {};
    filtered.forEach(user => {
      if (user.state) {
        stateCounts[user.state] = (stateCounts[user.state] || 0) + 1;
      }
    });

    this.stateChartData = {
      labels: Object.keys(stateCounts),
      datasets: [
        {
          label: 'Users per State',
          data: Object.values(stateCounts),
          backgroundColor: Object.keys(stateCounts).map((_, i) =>
            `hsl(${(i * 360) / Object.keys(stateCounts).length}, 70%, 60%)`
          ),
          hoverBackgroundColor: Object.keys(stateCounts).map((_, i) =>
            `hsl(${(i * 360) / Object.keys(stateCounts).length}, 70%, 45%)`
          )
        }
      ]
    };
  }

  get filteredUsers(): UserModel[] {
    return this.users.filter(user => {
      const matchesGender = this.selectedGender ? user.gender === this.selectedGender : true;
      const matchesRole = this.selectedRole ? user.role === this.selectedRole : true;
      const matchesState = this.selectedState ? user.state === this.selectedState : true;
      const matchesSearch = this.searchText
        ? (`${user.firstName} ${user.lastName}`.toLowerCase().includes(this.searchText.toLowerCase()))
        : true;

      return matchesGender && matchesRole && matchesState && matchesSearch;
    });
  }

  handleAddUser(){
    this.router.navigateByUrl("/add-user");
  }
}