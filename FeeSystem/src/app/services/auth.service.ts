import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
private LoggedIn = false;
  private apiUrl = 'https://localhost:44311/api';

  constructor(private http: HttpClient,private router:Router) { }

  isLoggedIn(): boolean {
    const token = localStorage.getItem('token');
    return !!token; 
  }
  isHost():boolean {
    const tenantId = localStorage.getItem('tenantId');
    if(tenantId== null){
      return true;
    }
    return false;
  }

  login(model:{ userNameOrEmailAddress: string; password: string; tenantId: string; rememberClient:boolean}): Observable<any> {
    this.LoggedIn=true;
    return this.http.post(`https://localhost:44311/api/TokenAuth/Authenticate`, {
      userNameOrEmailAddress: model.userNameOrEmailAddress,
      password: model.password,
      rememberClient: model.rememberClient,
      tenantId: model.tenantId || null
    });
  }

  saveTenant(tenantId: number): void {
    localStorage.setItem('tenantId', tenantId.toString());
  }

  getToken(): string {
    return localStorage.getItem('token') || '';
  }

  getTenant(): string | null {
    return localStorage.getItem('tenantId');
  }

  istenantAvailable( tenantId: string): Observable<any>{
    return this.http.post(`${this.apiUrl}/services/app/Account/IsTenantAvailable`,{tenancyName: tenantId} );
  }

    logout(){
      this.LoggedIn=false;
      localStorage.removeItem('token');
      localStorage.removeItem('tenantId')
      localStorage.removeItem('role')
      this.router.navigate(['login']);
    }
}