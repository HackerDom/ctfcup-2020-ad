data "yandex_compute_image" "ubuntu-with-docker" {
  family = "ubuntu-with-docker"
  folder_id = "<data deleted>"
}

locals {
  ubuntu-with-docker-id = data.yandex_compute_image.ubuntu-with-docker.id
}

locals {
  jury_subnet = cidrsubnet(var.base_subnet, 8, 0)
  deployer_ip = cidrhost(local.jury_subnet, 5)
}

resource "yandex_compute_instance" "deployer" {
  name = "deployer"

  lifecycle {
    prevent_destroy = true
  }

  resources {
    cores  = 4
    memory = 8
    core_fraction = 20
  }

  boot_disk {
    auto_delete = false

    initialize_params {
      image_id = local.ubuntu-with-docker-id
      size = 100
      type = "network-hdd"
    }
  }

  network_interface {
    subnet_id = yandex_vpc_subnet.dev-subnet.id
    ip_address = local.deployer_ip
    nat       = true
  }

  metadata = {
    ssh-keys = "ubuntu:${file("${path.root}/../../teams/for_devs.ssh_key.pub")}"
  }
}

output "deployer_fip" {
  value = yandex_compute_instance.deployer.network_interface.0.nat_ip_address
}


