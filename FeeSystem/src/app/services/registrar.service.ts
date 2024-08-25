import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class RegistrarService {
  private apiUrl = 'https://localhost:44311/api/services/app';

  constructor(private http: HttpClient) {}

  getStudent(): Observable<any>{
    return this.http.get(`${this.apiUrl}/Student/GetAllStudents`)
  }

  getclass(): Observable<any>{
    return this.http.get(`${this.apiUrl}/Student/GetAllClasses`)
  }

  getschool(): Observable<any>{
    return this.http.get(`${this.apiUrl}/Tenant/GetAll`)
  }

  addStudent(newStudent: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/Student/CreateStudent`, newStudent)
  }

  addNewSchool(addNewSchool: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/Tenant/Create`, addNewSchool)
  }

  updateStudent(upStudent: any): Observable<any> {
    return this.http.put(`${this.apiUrl}/Student/UpdateStudent`, upStudent)
  }

  addClass(newClass: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/Student/CreateOrUpdateClass`, newClass)
  }

  deleteStudent(delstudent: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/Student/DeleteStudent?id=${delstudent}`)
  }

  updatefeeforstudent(upfee: any): Observable<any> {
    return this.http.put(`${this.apiUrl}/Student/UpdateStudentFee`, upfee);
  }

  updateSchool(upschool: any): Observable<any>{
      return this.http.put(`${this.apiUrl}/Tenant/Update`, upschool);
  }

  deleteclass(delclass: number): Observable<any>{
     return this.http.delete(`${this.apiUrl}/Student/DeleteClass?id=${delclass}`)
  }

  deleteSchool(delschool: number): Observable<any>{
    return this.http.delete(`${this.apiUrl}/Tenant/Delete?Id=${delschool}`)
  }
}
