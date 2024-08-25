import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { FeeService } from '../../../services/fee.service';

@Component({
  selector: 'app-feetype',
  templateUrl: './feetype.component.html',
  styleUrl: './feetype.component.css'
})
export class FeetypeComponent {

  model: any = []
  adfeetype: any = {}
  delfeetype:number=0
  feetypemodel: any = []
  upfeetype: any = {}

  constructor(private router:Router, private feeService: FeeService){
    this.getallfeetype()
    }

    getallfeetype(){
      this.feeService.getallfeetype().subscribe(response =>{
        console.log(response.result);
        this.feetypemodel = response.result;
      });
    }

  addfeetype(){
    this.feeService.addfeetype(this.adfeetype).subscribe(response =>{
      alert('Fee Type Added Successfully');
      window.location.reload();
      this.router.navigate(['feetype']);
    }, error => {
      console.error(error);
      alert('Failed to add Fee');
    });
  }

  deletefeetype(delfeetype:number){
    this.feeService.deletefeetype(delfeetype).subscribe(response =>{
      alert('Fee Type Deleted Successfully');
      window.location.reload();
      this.router.navigate(['feetype']);
    }, error => {
      console.error(error);
      alert('Failed to Delete Fee');
    });
  }

  updatefeetype(){
    this.feeService.updatefeetype(this.upfeetype).subscribe(response => {
      alert('Update added successfully');
      window.location.reload();
     this.router.navigate(['school']);
    }, error => {
      console.error(error);
      alert('Failed to Update Fee Type');
    });
  }
}
