class MyApp {
    current = '';
    
    display(ctx) {
        this.current = ctx.inputText('Enter a value', this.current);
        ctx.text("You've entered: " + this.current);
    }
}

runApp('Test', new MyApp());