import { Component } from '@angular/core';
import { RegistrarService } from '../../../services/registrar.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-school',
  templateUrl: './school.component.html',
  styleUrl: './school.component.css'
})
export class SchoolComponent {

  models: any = []
  newSchool: any = {
    connectionString:'Server=tcp:localhost,1433;Database=test2;User ID=SA;Password=Password123;Encrypt=true;TrustServerCertificate=true;Connection Timeout=30;',
    isActive: true
  }
  delschool: number=0;
  upschool: any = {
    isActive:true
  }

  constructor(private registrarService: RegistrarService, private router: Router){
    this.getschool()
  }

  getschool(){
    this.registrarService.getschool().subscribe(response =>{
      this.models = response.result.items
    })
  }

  addNewSchool(){
    this.registrarService.addNewSchool(this.newSchool).subscribe(response =>{
      alert('School Added Successfully');
      window.location.reload();
      this.router.navigate(['student']);
    }, error => {
      console.error(error);
      alert('Failed to add School');
    });
  }

  deleteschool(delschool: number){
    this.registrarService.deleteSchool(delschool).subscribe(response => {
      alert('School deleted successfully');
      window.location.reload();
    }, error => {
      console.error(error);
      alert('Failed to delete Student');
    });

  }

  updateSchool(){
    this.registrarService.updateSchool(this.upschool).subscribe(response => {
      alert('Update added successfully');
      window.location.reload();
     this.router.navigate(['school']);
    }, error => {
      console.error(error);
      alert('Failed to Update School');
    });
  }

}
