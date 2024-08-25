import { Component } from '@angular/core';
import { AuthService } from '../../services/auth.service';
import { jwtDecode } from 'jwt-decode';
import { Router } from '@angular/router';
import { AppTenantAvailabilityState } from '../../services/tenant.service';
import { Token } from '@angular/compiler';


@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
  

export class LoginComponent {
  model  = {
    userNameOrEmailAddress: '' ,
    password: '' ,
    tenantId: '' ,
    rememberClient:true
  }


constructor(private authService: AuthService, private router: Router){}

istenantAvailable() {


  // If tenantId is empty or null, treat this as a host login
  if (!this.model.tenantId) {
    this.login(); // Directly proceed to login as a host
    return;
  }

  this.authService.istenantAvailable(this.model.tenantId).subscribe(
    (response) => {
      console.log(response)
      switch (response.result.state) {
        case AppTenantAvailabilityState.Available:
          this.authService.saveTenant(response.result.tenantId)
          this.login();
          return;

        case AppTenantAvailabilityState.InActive:
          alert('Tenant is not active');
          break;

        case AppTenantAvailabilityState.NotFound:
          alert('Tenant not found');
          break;
      }
    },
    (error) => {
      console.error(error);
      alert('School not available');
    }
  );
}

login(){
  this.authService.login(this.model).subscribe((response) => {
    console.log(response);
    localStorage.setItem('token', response.result.accessToken);
    const decodedToken: any = jwtDecode(response.result.accessToken);
    const role=decodedToken['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'];
    localStorage.setItem('role',role)
    if (role === 'Admin') {
      this.router.navigate(['student']);
    } else {
      this.router.navigate(['login']);
    }

    alert('Login successful');
  }, (error) => {
    console.error(error);
    alert('Login failed');
  });
}


}




