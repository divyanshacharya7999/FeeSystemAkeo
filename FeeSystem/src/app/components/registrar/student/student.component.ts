import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { RegistrarService } from '../../../services/registrar.service';
import { AuthService } from '../../../services/auth.service';


@Component({
  selector: 'app-student',
  templateUrl: './student.component.html',
  styleUrl: './student.component.css'
})


export class StudentComponent {
  model: any = [];
  searchId: string = '';
  selectedStudentId: number | null = null;
  
  newStudent: any = {
    dateOfBirth: "2024-08-20T13:01:22.992Z"
  }
  upStudent: any = { 
    dateOfBirth: "2024-08-20T13:01:22.992Z"
  }
 
  delstudent: string='';
  
  isHostLogin : any = false;
  constructor (private router:Router, private registrarService: RegistrarService,public authService:AuthService){
    this.getStudent();
    this.isHostLogin = this.authService.isHost();
  }

  studentfee(studentId: any){
    localStorage.setItem('studentId',studentId)
    this.router.navigate(['studentfees'])
  }

  getStudent(){
    this.registrarService.getStudent().subscribe(data => {
      console.log(data);
      this.model = data.result;
    });
  }
  getschool(){
    this.router.navigate(['school'])
  }

  deleteStudent(delstudent: any){
    this.registrarService.deleteStudent(delstudent).subscribe(response => {
      alert('Student deleted successfully');
      window.location.reload();
    }, error => {
      console.error(error);
      alert('Failed to delete Student');
    });
  }

  addStudent(){
    this.registrarService.addStudent(this.newStudent).subscribe(response => {
      alert('Student added successfully');
      window.location.reload();
      this.router.navigate(['student']);
    }, error => {
      console.error(error);
      alert('Failed to add Student');
    });
  }

  getclass(){
    this.router.navigate(['class'])
  }

  updateStudent(){
    this.registrarService.updateStudent(this.upStudent).subscribe(response => {
      debugger;
      alert('Update added successfully');
      window.location.reload();
     this.router.navigate(['student']);
    }, error => {
      console.error(error);
      alert('Failed to Update Student');
    });
  }

    searchStudent() {
      if (this.searchId.trim()) {
        const idToSearch = this.searchId.trim();
        const foundStudent = this.model.find((student: { studentId: string }) => student.studentId === idToSearch);
        if (foundStudent) {
          this.selectedStudentId = foundStudent.studentId;
        } else {
          this.selectedStudentId = null;
        }
      } else {
        this.selectedStudentId = null;
      }
    }
}
