class App {
    text = 'Not clicked yet';
    checked = true;
    
    // menu(menuBar) {
    //     menuBar.beginMenu('File', menu => {
    //         if (menu.menuItem('One')) {
    //             this.text = "clicked one";
    //         }
    //         if (menu.menuItem('Two')) {
    //             this.text = "clicked two";
    //         }
    //         menu.beginMenu('Options', menu => {
    //             this.checked = menu.checkbox('Should ded?', this.checked);
    //         })
    //     })
    //     menuBar.beginMenu('Edit', menu => {
    //         this.checked = menu.checkbox('Edit yes?', this.checked);
    //     })
    // }
    
    display(ctx) {
        ctx.text(this.text);
    }
}

runApp('Main', new App())