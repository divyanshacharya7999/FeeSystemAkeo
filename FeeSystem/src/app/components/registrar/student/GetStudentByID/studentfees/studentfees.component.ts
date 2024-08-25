import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { FeeService } from '../../../../../services/fee.service';
import { RegistrarService } from '../../../../../services/registrar.service';
import { AuthService } from '../../../../../services/auth.service';

@Component({
  selector: 'app-studentfees',
  templateUrl: './studentfees.component.html',
  styleUrl: './studentfees.component.css'
})
export class StudentfeesComponent {
  model: any = []
  upfee: any = {}
 

  constructor(private router: Router, private feeService : FeeService, private registrarService : RegistrarService){
    this.getFeeForStudent();
    
  }
  getFeeForStudent(){
    const studentId  = localStorage.getItem('studentId')

   this.feeService.getfeeForStudent(studentId).subscribe(data => {
    console.log(data);
    this.model = data.result;
  });
  }

  updatefee1type(){
    this.registrarService.updatefeeforstudent(this.upfee).subscribe(data=>{
      alert('Update added successfully');
      window.location.reload();
     this.router.navigate(['studentfees']);
    }, error => {
      console.error(error);
      alert('Failed to Update Student Fee');
    });
    
  }
  get TotalAmount(): number {
  return this.model.reduce((sum: number, current: { amountPerPeriod: number }) => {
    return sum + current.amountPerPeriod;
  }, 0); // Initial value of sum is 0
}

}
