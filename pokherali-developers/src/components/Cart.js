import React, { useState } from "react"
import { Trash2 } from 'lucide-react'

export default function Cart() {
  // Sample cart items
  const [cartItems, setCartItems] = useState([
    { id: 1, title: "The Great Adventure", price: 20, quantity: 1 },
    { id: 2, title: "Mystery of the Blue Lake", price: 15, quantity: 1 },
  ])

  const removeItem = (id) => {
    setCartItems(cartItems.filter((item) => item.id !== id))
  }

  const updateQuantity = (id, quantity) => {
    if (quantity < 1) return
    setCartItems(cartItems.map((item) => (item.id === id ? { ...item, quantity } : item)))
  }

  const subtotal = cartItems.reduce((sum, item) => sum + item.price * item.quantity, 0)
  const shipping = subtotal > 0 ? 5.99 : 0
  const total = subtotal + shipping

  return (
    <div className="bg-white p-4 rounded-lg shadow-md">
      <h2 className="text-xl font-semibold mb-4">Your Cart</h2>

      {cartItems.length === 0 ? (
        <p className="text-gray-500 text-center py-4">Your cart is empty</p>
      ) : (
        <>
          <div className="space-y-4 mb-4">
            {cartItems.map((item) => (
              <div key={item.id} className="flex items-start gap-2 pb-3 border-b">
                <div className="flex-1">
                  <h3 className="font-medium">{item.title}</h3>
                  <p className="text-gray-600">${item.price.toFixed(2)}</p>
                  <div className="flex items-center mt-1">
                    <button
                      onClick={() => updateQuantity(item.id, item.quantity - 1)}
                      className="w-6 h-6 flex items-center justify-center border rounded-md"
                    >
                      -
                    </button>
                    <span className="mx-2">{item.quantity}</span>
                    <button
                      onClick={() => updateQuantity(item.id, item.quantity + 1)}
                      className="w-6 h-6 flex items-center justify-center border rounded-md"
                    >
                      +
                    </button>
                  </div>
                </div>
                <button
                  onClick={() => removeItem(item.id)}
                  className="text-red-500 hover:text-red-700"
                  aria-label="Remove item"
                >
                  <Trash2 size={18} />
                </button>
              </div>
            ))}
          </div>

          <div className="space-y-2 text-sm mb-4">
            <div className="flex justify-between">
              <span>Subtotal</span>
              <span>${subtotal.toFixed(2)}</span>
            </div>
            <div className="flex justify-between">
              <span>Shipping</span>
              <span>${shipping.toFixed(2)}</span>
            </div>
            <div className="flex justify-between font-semibold text-base pt-2 border-t">
              <span>Total</span>
              <span>${total.toFixed(2)}</span>
            </div>
          </div>

          <button className="w-full bg-blue-600 text-white py-2 rounded-md hover:bg-blue-700 transition-colors">
            Proceed to Checkout
          </button>
        </>
      )}
    </div>
  )
}
