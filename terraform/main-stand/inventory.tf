### The Ansible inventory file
resource "local_file" "AnsibleInventory" {
  content = templatefile("${path.module}/templates/inventory.tmpl", {
    cs-main-ip = yandex_compute_instance.cs-main.network_interface.0.ip_address,
    cs-worker-ip = yandex_compute_instance.cs-worker.network_interface.0.ip_address,
    proxy-ip = yandex_compute_instance.proxy[*].network_interface.0.ip_address
  })
  filename = "../inventory"
}
