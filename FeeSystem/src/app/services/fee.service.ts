import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class FeeService {

  private apiUrl = 'https://localhost:44311/api';

  constructor(private http: HttpClient) { }

  getfeeForStudent(studentId: any): Observable<any>{
    return this.http.get(`${this.apiUrl}/services/app/Student/GetFeesForStudent?studentId=${studentId}`)
  }
  updatefeeforstudent(model:any): Observable<any>{
    return this.http.get(`${this.apiUrl}/services/app/Student/UpdateStudentFee`,model)
  }

  getAllFee(): Observable<any>{
    return this.http.get(`${this.apiUrl}/services/app/Fee/GetAll`)
  }

  getallpaymentplan():Observable<any>{
    return this.http.get(`${this.apiUrl}/services/app/Fee/GetAllPaymentPlans`)
  }

  addpaymentplan(payplan: any): Observable<any>{
    return this.http.post(`${this.apiUrl}/services/app/Fee/CreatePaymentPlan`, payplan)
  }
  getallfeetype():Observable<any>{
    return this.http.get(`${this.apiUrl}/services/app/Fee/GetAllFeeTypes`)
  }

  addfee(addfee: any): Observable<any>{
    return this.http.post(`${this.apiUrl}/services/app/Fee/CreateOrUpdate`, addfee)
  }

  updatefeetype(upfeetype:any):Observable<any> {
    return this.http.put(`${this.apiUrl}/services/app/Fee/UpdateFeeType`, upfeetype)}

  addfeetype(adfeetype:any): Observable<any>{
    return this.http.post(`${this.apiUrl}/services/app/Fee/CreateFeeType`, adfeetype)
  }
  updatefee(upfee: any): Observable<any> {
    return this.http.put(`${this.apiUrl}/assign-role`, upfee)
  }  
  // krni hai

  deletefee(delfee: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/services/app/Fee/Delete?id=${delfee}`)
  }
  deletefeetype(delfeetype: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/services/app/Fee/DeleteFeeType?input=${delfeetype}`)
  }
  deletepaymentplan(delpaymentplan: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/services/app/Fee/DeletePaymentPlan?Id=${delpaymentplan}`)
  }
  updatepaymentplan(upplan:any):Observable<any> {
    return this.http.put(`${this.apiUrl}/services/app/Fee/UpdatePaymentPlan`, upplan)}
}
