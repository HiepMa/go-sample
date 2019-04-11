package main
import (
	"fmt"
	"math"
)
func s1_var(){
	// var age int // variable declaration
//    fmt.Println("my age is ", age)
    // age = 29 //assignment
    // fmt.Println("my age is", age)
    // age = 54 //assignment
	// fmt.Println("my new age is", age)

	// var age = 29 // type will be inferred
	// fmt.Println("my age is", age)
	
	// var width, height int = 100, 50 //declaring multiple variables
	// fmt.Println("width is", width, "height is", height)
	// var (
    //     name   = "naveen"
    //     age    = 29
    //     height int
    // )
	// fmt.Println("my name is", name, ", age is", age, "and height is", height)

	// a, b := 20, 30 // declare variables a and b
    // fmt.Println("a is", a, "b is", b)
    // b, c := 40, 50 // b is already declared but c is new
    // fmt.Println("b is", b, "c is", c)
    // b, c = 80, 90 // assign new values to already declared variables b and c
	// fmt.Println("changed b is", b, "c is", c)
	
	a, b := 145.8, 543.8
	c := math.Min(a, b)
	fmt.Println("minimum value is ", c)
}
