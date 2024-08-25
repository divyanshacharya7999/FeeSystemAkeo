import { Component } from '@angular/core';
import { RegistrarService } from '../../../services/registrar.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-class',
  templateUrl: './class.component.html',
  styleUrl: './class.component.css'
})
export class ClassComponent {

  models: any = []
  addclass: any = {}
  delclass: number=0;

  constructor(private registrarService: RegistrarService, private router: Router){
    this.getclass()
  }

  getclass(){
    this.registrarService.getclass().subscribe(response =>{
      this.models = response.result
    })
  }

  addclasses(){
    this.registrarService.addClass(this.addclass).subscribe(response =>{
      alert('Class Added Successfully');
      window.location.reload();
      this.router.navigate(['class']);
    }, error => {
      console.error(error);
      alert('Failed to add class');
    });
  }

  deleteclass(delclass:number){
    this.registrarService.deleteclass(delclass).subscribe(response =>{
      alert('Class Deleted Successfully');
      window.location.reload();
      this.router.navigate(['class']);
    }, error => {
      console.error(error);
      alert('Failed to Delete class');
    });
  }

}
